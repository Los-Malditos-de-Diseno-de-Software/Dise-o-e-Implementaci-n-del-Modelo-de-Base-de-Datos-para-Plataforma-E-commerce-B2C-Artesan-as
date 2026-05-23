using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Productos.CreateProducto;

public record CreateProductoCommand(
    Guid ArtesanoId,
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    bool EsUnico,
    // Imagen recibida como bytes + content type desde el controlador (multipart/form-data)
    byte[]? ImagenData,
    string? ImagenContentType
) : IRequest<Result<ProductoDto>>;

public class CreateProductoCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateProductoCommand, Result<ProductoDto>>
{
    public async Task<Result<ProductoDto>> Handle(
        CreateProductoCommand request,
        CancellationToken cancellationToken)
    {
        var artesanoExiste = await uow.Artesanos.ExistsAsync(request.ArtesanoId, cancellationToken);
        if (!artesanoExiste)
            return Result<ProductoDto>.Fail($"Artesano {request.ArtesanoId} no encontrado.");

        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            ArtesanoId = request.ArtesanoId,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Precio = request.Precio,
            Stock = request.Stock,
            EsUnico = request.EsUnico,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "system",
            UpdatedBy = "system"
        };

        if (request.ImagenData is not null && request.ImagenContentType is not null)
        {
            producto.Imagenes.Add(new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductoId = producto.Id,
                ImageData = request.ImagenData,
                ContentType = request.ImagenContentType,
                EsPrincipal = true
            });
        }

        await uow.Productos.AddAsync(producto, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        string? base64 = request.ImagenData is not null && request.ImagenContentType is not null
            ? $"data:{request.ImagenContentType};base64,{Convert.ToBase64String(request.ImagenData)}"
            : null;

        return Result<ProductoDto>.Ok(new ProductoDto
        {
            Id = producto.Id,
            ArtesanoId = producto.ArtesanoId,
            ArtesanoNombre = string.Empty,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Stock = producto.Stock,
            EsUnico = producto.EsUnico,
            CreatedAt = producto.CreatedAt,
            ImagenBase64 = base64
        }, "Producto creado correctamente.");
    }
}
