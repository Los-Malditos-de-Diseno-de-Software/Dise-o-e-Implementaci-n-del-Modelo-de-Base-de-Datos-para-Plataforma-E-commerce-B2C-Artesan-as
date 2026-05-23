using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Cart.GetCart;

public record GetCartQuery(Guid SessionId) : IRequest<Result<CartDto>>;

public class GetCartQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        var cart = await uow.Cart.GetOrCreateBySessionAsync(request.SessionId, cancellationToken);

        var dto = new CartDto
        {
            Id = cart.Id,
            SessionId = cart.SessionId,
            Items = cart.Items.Select(item =>
            {
                var imagenPrincipal = item.Producto?.Imagenes.FirstOrDefault(i => i.EsPrincipal)
                                      ?? item.Producto?.Imagenes.FirstOrDefault();

                string? base64 = null;
                if (imagenPrincipal is not null)
                    base64 = $"data:{imagenPrincipal.ContentType};base64,{Convert.ToBase64String(imagenPrincipal.ImageData)}";

                return new CartItemDto
                {
                    Id = item.Id,
                    ProductoId = item.ProductoId,
                    ProductoNombre = item.Producto?.Nombre ?? string.Empty,
                    ProductoImagenBase64 = base64,
                    Cantidad = item.Cantidad,
                    PrecioUnitarioCongelado = item.PrecioUnitarioCongelado
                };
            }).ToList()
        };

        return Result<CartDto>.Ok(dto);
    }
}
