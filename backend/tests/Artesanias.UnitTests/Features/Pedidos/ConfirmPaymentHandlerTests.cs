using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Artesanias.Application.Features.Pedidos.ConfirmPayment;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Artesanias.UnitTests.Features.Pedidos;

public class ConfirmPaymentHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IPaymentTransactionRepository> _paymentTxRepoMock;
    private readonly Mock<IOrderRepository> _ordersRepoMock;
    private readonly Mock<IProductoRepository> _productosRepoMock;
    private readonly Mock<ICartRepository> _cartRepoMock;
    private readonly ConfirmPaymentCommandHandler _handler;

    public ConfirmPaymentHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _paymentTxRepoMock = new Mock<IPaymentTransactionRepository>();
        _ordersRepoMock = new Mock<IOrderRepository>();
        _productosRepoMock = new Mock<IProductoRepository>();
        _cartRepoMock = new Mock<ICartRepository>();

        _uowMock.Setup(x => x.PaymentTransactions).Returns(_paymentTxRepoMock.Object);
        _uowMock.Setup(x => x.Orders).Returns(_ordersRepoMock.Object);
        _uowMock.Setup(x => x.Productos).Returns(_productosRepoMock.Object);
        _uowMock.Setup(x => x.Cart).Returns(_cartRepoMock.Object);

        _handler = new ConfirmPaymentCommandHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenPaymentTransactionNotFound()
    {
        // Arrange
        var command = new ConfirmPaymentCommand("sess_123", "pi_123", "raw_payload");

        _paymentTxRepoMock.Setup(x => x.GetByStripeSessionIdAsync("sess_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentTransaction)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("PaymentTransaction no encontrada");
        
        _uowMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        // Note: early return does not throw, hence no rollback in current handler design
        _uowMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkIdempotent_WhenPaymentIsAlreadyConfirmed()
    {
        // Arrange
        var command = new ConfirmPaymentCommand("sess_123", "pi_123", "raw_payload");
        var paymentTx = new PaymentTransaction { EstadoPago = EstadosPago.Pagado };

        _paymentTxRepoMock.Setup(x => x.GetByStripeSessionIdAsync("sess_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentTx);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("Pago ya confirmado previamente");
        _uowMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenOrderNotFound()
    {
        // Arrange
        var command = new ConfirmPaymentCommand("sess_123", "pi_123", "raw_payload");
        var orderId = Guid.NewGuid();
        var paymentTx = new PaymentTransaction { OrderId = orderId, EstadoPago = EstadosPago.Pendiente };

        _paymentTxRepoMock.Setup(x => x.GetByStripeSessionIdAsync("sess_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentTx);

        _ordersRepoMock.Setup(x => x.GetByIdWithItemsAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Order");
        result.Message.Should().Contain("no encontrada");
        _uowMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldExecuteACIDTransactionSuccessfully_UpdatingStatesUpdatingStockDeletingCart()
    {
        // Arrange
        var command = new ConfirmPaymentCommand("sess_123", "pi_123", "raw_payload");
        var orderId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var paymentTx = new PaymentTransaction { OrderId = orderId, EstadoPago = EstadosPago.Pendiente };
        
        var prodId1 = Guid.NewGuid();
        var prodId2 = Guid.NewGuid();
        var orderItems = new List<OrderItem>
        {
            new() { ProductoId = prodId1, Cantidad = 2 },
            new() { ProductoId = prodId2, Cantidad = 1 }
        };
        var order = new Order { Id = orderId, UsuarioId = usuarioId, EstadoPedido = EstadosPedido.Pendiente, Items = orderItems };

        var prod1 = new Producto { Id = prodId1, Nombre = "Toro", Stock = 10 };
        var prod2 = new Producto { Id = prodId2, Nombre = "Chakana", Stock = 5 };

        _paymentTxRepoMock.Setup(x => x.GetByStripeSessionIdAsync("sess_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentTx);

        _ordersRepoMock.Setup(x => x.GetByIdWithItemsAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _productosRepoMock.Setup(x => x.GetByIdAsync(prodId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(prod1);
        _productosRepoMock.Setup(x => x.GetByIdAsync(prodId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(prod2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("Pago confirmado");

        // Verify status updates
        paymentTx.EstadoPago.Should().Be(EstadosPago.Pagado);
        paymentTx.StripePaymentIntentId.Should().Be("pi_123");
        order.EstadoPedido.Should().Be(EstadosPedido.Pagado);

        // Verify stock decrements
        prod1.Stock.Should().Be(8);
        prod2.Stock.Should().Be(4);

        // Verify repository updates
        _paymentTxRepoMock.Verify(x => x.Update(paymentTx), Times.Once);
        _ordersRepoMock.Verify(x => x.Update(order), Times.Once);
        _productosRepoMock.Verify(x => x.Update(prod1), Times.Once);
        _productosRepoMock.Verify(x => x.Update(prod2), Times.Once);
        _cartRepoMock.Verify(x => x.DeleteByUsuarioIdAsync(usuarioId, It.IsAny<CancellationToken>()), Times.Once);
        
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldRollbackAndThrow_WhenAnErrorOccursDuringExecution()
    {
        // Arrange
        var command = new ConfirmPaymentCommand("sess_123", "pi_123", "raw_payload");
        var orderId = Guid.NewGuid();
        var paymentTx = new PaymentTransaction { OrderId = orderId, EstadoPago = EstadosPago.Pendiente };
        
        var prodId = Guid.NewGuid();
        var orderItems = new List<OrderItem>
        {
            new() { ProductoId = prodId, Cantidad = 2 }
        };
        var order = new Order { Id = orderId, EstadoPedido = EstadosPedido.Pendiente, Items = orderItems };

        _paymentTxRepoMock.Setup(x => x.GetByStripeSessionIdAsync("sess_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentTx);

        _ordersRepoMock.Setup(x => x.GetByIdWithItemsAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Force an exception by returning null for product in order item loop
        _productosRepoMock.Setup(x => x.GetByIdAsync(prodId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Producto)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Producto {prodId} no encontrado al confirmar pago.");

        _uowMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
