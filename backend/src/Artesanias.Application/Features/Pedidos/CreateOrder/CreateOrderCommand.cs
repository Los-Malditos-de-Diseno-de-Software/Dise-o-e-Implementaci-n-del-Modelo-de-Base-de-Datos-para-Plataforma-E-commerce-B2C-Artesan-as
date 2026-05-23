using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Application.Interfaces;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Pedidos.CreateOrder;

public record CreateOrderCommand(
    Guid SessionId,
    Guid UsuarioId,
    string DireccionEnvio,
    // URLs de retorno para Stripe Checkout
    string SuccessUrl,
    string CancelUrl
) : IRequest<Result<CreateOrderResponseDto>>;

/// <summary>
/// Este handler SOLO crea la Order + PaymentTransaction en estado Pendiente
/// y genera la sesión de Stripe Checkout.
/// El stock NO se descuenta aquí — eso ocurre en ConfirmPaymentCommand (webhook ACID).
/// </summary>
public class CreateOrderCommandHandler(IUnitOfWork uow, IStripeService stripeService)
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponseDto>>
{
    public async Task<Result<CreateOrderResponseDto>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verificar que el usuario existe
        var usuario = await uow.Usuarios.GetByIdAsync(request.UsuarioId, cancellationToken);
        if (usuario is null)
            return Result<CreateOrderResponseDto>.Fail("Usuario no encontrado.");

        // 2. Obtener el carrito
        var cart = await uow.Cart.GetOrCreateBySessionAsync(request.SessionId, cancellationToken);
        if (!cart.Items.Any())
            return Result<CreateOrderResponseDto>.Fail("El carrito está vacío.");

        // 3. Verificar stock disponible para todos los ítems antes de proceder
        foreach (var cartItem in cart.Items)
        {
            var producto = await uow.Productos.GetByIdAsync(cartItem.ProductoId, cancellationToken);
            if (producto is null)
                return Result<CreateOrderResponseDto>.Fail($"Producto {cartItem.ProductoId} no encontrado.");
            if (producto.Stock < cartItem.Cantidad)
                return Result<CreateOrderResponseDto>.Fail(
                    $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}.");
        }

        // 4. Crear la Order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            DireccionEnvio = request.DireccionEnvio,
            EstadoPedido = EstadosPedido.Pendiente,
            Total = cart.Items.Sum(i => i.Cantidad * i.PrecioUnitarioCongelado),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = usuario.Email,
            UpdatedBy = usuario.Email
        };

        // 5. Agregar OrderItems desde el carrito
        foreach (var cartItem in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductoId = cartItem.ProductoId,
                Cantidad = cartItem.Cantidad,
                PrecioUnitario = cartItem.PrecioUnitarioCongelado
            });
        }

        await uow.Orders.AddAsync(order, cancellationToken);

        // 6. Crear sesión de Stripe Checkout
        var stripeLineItems = cart.Items.Select(i => new StripeLineItem(
            i.Producto?.Nombre ?? "Producto",
            i.PrecioUnitarioCongelado,
            i.Cantidad
        )).ToList();

        var stripeSession = await stripeService.CreateCheckoutSessionAsync(
            order.Id,
            stripeLineItems,
            request.SuccessUrl,
            request.CancelUrl,
            cancellationToken);

        // 7. Crear PaymentTransaction inicial (Pendiente)
        var paymentTx = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            MetodoPago = "Stripe",
            EstadoPago = EstadosPago.Pendiente,
            StripeSessionId = stripeSession.SessionId,
            StripePaymentIntentId = string.Empty,
            ReferenciaPasarela = stripeSession.SessionId,
            PayloadPasarela = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await uow.PaymentTransactions.AddAsync(paymentTx, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<CreateOrderResponseDto>.Ok(new CreateOrderResponseDto
        {
            OrderId = order.Id,
            StripeCheckoutUrl = stripeSession.CheckoutUrl,
            StripeSessionId = stripeSession.SessionId
        }, "Orden creada. Redirige al usuario a la URL de Stripe Checkout.");
    }
}

/// <summary>Contrato con la capa de Infrastructure para Stripe</summary>
public record StripeLineItem(string Nombre, decimal PrecioUnitario, int Cantidad);
public record StripeSessionResult(string SessionId, string CheckoutUrl);
