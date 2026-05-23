using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Pedidos.GetAllOrders;

public record GetAllOrdersQuery : IRequest<Result<List<OrderDto>>>;

public class GetAllOrdersQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAllOrdersQuery, Result<List<OrderDto>>>
{
    public async Task<Result<List<OrderDto>>> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await uow.Orders.GetAllWithDetailsAsync(cancellationToken);

        var dtos = orders.Select(order => new OrderDto
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
        }).ToList();

        return Result<List<OrderDto>>.Ok(dtos);
    }
}
