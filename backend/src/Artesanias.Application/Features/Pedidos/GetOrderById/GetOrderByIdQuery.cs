using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Pedidos.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId, Guid UsuarioId) : IRequest<Result<OrderDto>>;

public class GetOrderByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await uow.Orders.GetByIdWithItemsAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result<OrderDto>.Fail("Orden no encontrada.");

        // Verificar que la orden pertenece al usuario (seguridad)
        if (order.UsuarioId != request.UsuarioId)
            return Result<OrderDto>.Fail("No tienes permisos para ver esta orden.");

        var dto = new OrderDto
        {
            Id = order.Id,
            UsuarioId = order.UsuarioId,
            Total = order.Total,
            EstadoPedido = order.EstadoPedido,
            DireccionEnvio = order.DireccionEnvio,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductoId = i.ProductoId,
                ProductoNombre = i.Producto?.Nombre ?? string.Empty,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                Subtotal = i.Cantidad * i.PrecioUnitario
            }).ToList(),
            Pago = order.Pago is not null ? new PaymentTransactionDto
            {
                Id = order.Pago.Id,
                MetodoPago = order.Pago.MetodoPago,
                EstadoPago = order.Pago.EstadoPago,
                StripeSessionId = order.Pago.StripeSessionId,
                CreatedAt = order.Pago.CreatedAt
            } : null
        };

        return Result<OrderDto>.Ok(dto);
    }
}
