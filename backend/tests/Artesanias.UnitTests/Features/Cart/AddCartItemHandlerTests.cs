using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artesanias.Application.DTOs;
using Artesanias.Application.Features.Cart.AddCartItem;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Artesanias.UnitTests.Features.Cart;

public class AddCartItemHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly AddCartItemCommandHandler _handler;

    public AddCartItemHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new AddCartItemCommandHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 2);
        
        _uowMock.Setup(x => x.Productos.GetByIdAsync(command.ProductoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Producto)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenProductStockIsInsufficient()
    {
        // Arrange
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 5);
        var producto = new Producto { Id = command.ProductoId, Stock = 2, Precio = 100 };

        _uowMock.Setup(x => x.Productos.GetByIdAsync(command.ProductoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Stock insuficiente");
    }

    [Fact]
    public async Task Handle_ShouldAddCartItem_WhenProductAndStockAreValidAndCartIsEmpty()
    {
        // Arrange
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 2);
        var producto = new Producto { Id = command.ProductoId, Nombre = "Textil Cusco", Stock = 10, Precio = 150 };
        var cart = new ShoppingCart { Id = Guid.NewGuid(), SessionId = command.SessionId, Items = new List<CartItem>() };

        _uowMock.Setup(x => x.Productos.GetByIdAsync(command.ProductoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        _uowMock.Setup(x => x.Cart.GetOrCreateBySessionAsync(command.SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("agregado al carrito");
        cart.Items.Should().HaveCount(1);
        cart.Items.First().ProductoId.Should().Be(command.ProductoId);
        cart.Items.First().Cantidad.Should().Be(command.Cantidad);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldIncreaseItemQuantity_WhenItemAlreadyExistsInCart()
    {
        // Arrange
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 2);
        var producto = new Producto { Id = command.ProductoId, Nombre = "Textil Cusco", Stock = 10, Precio = 150 };
        
        var cartItem = new CartItem { ProductoId = command.ProductoId, Cantidad = 3, PrecioUnitarioCongelado = 150 };
        var cart = new ShoppingCart { Id = Guid.NewGuid(), SessionId = command.SessionId, Items = new List<CartItem> { cartItem } };

        _uowMock.Setup(x => x.Productos.GetByIdAsync(command.ProductoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        _uowMock.Setup(x => x.Cart.GetOrCreateBySessionAsync(command.SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        cart.Items.Should().HaveCount(1);
        cart.Items.First().Cantidad.Should().Be(5); // 3 + 2
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenNewQuantityExceedsStock()
    {
        // Arrange
        var command = new AddCartItemCommand(Guid.NewGuid(), Guid.NewGuid(), 4);
        var producto = new Producto { Id = command.ProductoId, Nombre = "Textil Cusco", Stock = 5, Precio = 150 };
        
        var cartItem = new CartItem { ProductoId = command.ProductoId, Cantidad = 2, PrecioUnitarioCongelado = 150 };
        var cart = new ShoppingCart { Id = Guid.NewGuid(), SessionId = command.SessionId, Items = new List<CartItem> { cartItem } };

        _uowMock.Setup(x => x.Productos.GetByIdAsync(command.ProductoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        _uowMock.Setup(x => x.Cart.GetOrCreateBySessionAsync(command.SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Stock insuficiente");
        cartItem.Cantidad.Should().Be(2); // no debió cambiar
    }
}
