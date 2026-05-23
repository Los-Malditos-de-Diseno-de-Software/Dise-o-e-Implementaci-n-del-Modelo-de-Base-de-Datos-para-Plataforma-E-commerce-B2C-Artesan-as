using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Productos.GetProductoById;

public record GetProductoByIdQuery(Guid Id) : IRequest<Result<ProductoDto>>;

public class GetProductoByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetProductoByIdQuery, Result<ProductoDto>>
{
    public async Task<Result<ProductoDto>> Handle(
        GetProductoByIdQuery request,
        CancellationToken cancellationToken)
    {
        var producto = await uow.Productos.GetByIdWithImagesAsync(request.Id, cancellationToken);
        if (producto is null)
            return Result<ProductoDto>.Fail($"Producto {request.Id} no encontrado.");

        var imagenPrincipal = producto.Imagenes.FirstOrDefault(i => i.EsPrincipal)
                              ?? producto.Imagenes.FirstOrDefault();

        string? base64 = null;
        if (imagenPrincipal is not null)
            base64 = $"data:{imagenPrincipal.ContentType};base64,{Convert.ToBase64String(imagenPrincipal.ImageData)}";

        return Result<ProductoDto>.Ok(new ProductoDto
        {
            Id = producto.Id,
            ArtesanoId = producto.ArtesanoId,
            ArtesanoNombre = producto.Artesano?.Nombre ?? string.Empty,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Stock = producto.Stock,
            EsUnico = producto.EsUnico,
            CreatedAt = producto.CreatedAt,
            ImagenBase64 = base64
        });
    }
}
