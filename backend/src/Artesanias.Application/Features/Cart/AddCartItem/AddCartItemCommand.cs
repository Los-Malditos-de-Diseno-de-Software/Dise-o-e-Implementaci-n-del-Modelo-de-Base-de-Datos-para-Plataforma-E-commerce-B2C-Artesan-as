using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Cart.AddCartItem;

public record AddCartItemCommand(
    Guid SessionId,
    Guid ProductoId,
    int Cantidad
) : IRequest<Result<CartDto>>;

public class AddCartItemCommandHandler(IUnitOfWork uow)
    : IRequestHandler<AddCartItemCommand, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var producto = await uow.Productos.GetByIdAsync(request.ProductoId, cancellationToken);
        if (producto is null)
            return Result<CartDto>.Fail($"Producto {request.ProductoId} no encontrado.");

        if (producto.Stock < request.Cantidad)
            return Result<CartDto>.Fail($"Stock insuficiente. Disponible: {producto.Stock}.");

        var cart = await uow.Cart.GetOrCreateBySessionAsync(request.SessionId, cancellationToken);

        // Si ya existe el mismo producto en el carrito, sumar cantidad
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductoId == request.ProductoId);
        if (existingItem is not null)
        {
            var nuevaCantidad = existingItem.Cantidad + request.Cantidad;
            if (producto.Stock < nuevaCantidad)
                return Result<CartDto>.Fail($"Stock insuficiente para agregar {request.Cantidad} más. Disponible: {producto.Stock}.");

            existingItem.Cantidad = nuevaCantidad;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                ShoppingCartId = cart.Id,
                ProductoId = request.ProductoId,
                Cantidad = request.Cantidad,
                PrecioUnitarioCongelado = producto.Precio // precio congelado al momento de agregar
            });
        }

        cart.UltimaActualizacion = DateTime.UtcNow;
        await uow.SaveChangesAsync(cancellationToken);

        // Retornar carrito actualizado
        var cartActualizado = await uow.Cart.GetOrCreateBySessionAsync(request.SessionId, cancellationToken);
        var dto = MapToCartDto(cartActualizado);

        return Result<CartDto>.Ok(dto, "Producto agregado al carrito.");
    }

    private static CartDto MapToCartDto(Domain.Entities.ShoppingCart cart) =>
        new()
        {
            Id = cart.Id,
            SessionId = cart.SessionId,
            Items = cart.Items.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductoId = item.ProductoId,
                ProductoNombre = item.Producto?.Nombre ?? string.Empty,
                Cantidad = item.Cantidad,
                PrecioUnitarioCongelado = item.PrecioUnitarioCongelado
            }).ToList()
        };
}
