using Artesanias.Application.Common;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Productos.UpdateProducto;

public record UpdateProductoCommand(
    Guid Id,
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    bool EsUnico,
    byte[]? NuevaImagenData,
    string? NuevaImagenContentType
) : IRequest<Result>;

public class UpdateProductoCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateProductoCommand, Result>
{
    public async Task<Result> Handle(
        UpdateProductoCommand request,
        CancellationToken cancellationToken)
    {
        var producto = await uow.Productos.GetByIdWithImagesAsync(request.Id, cancellationToken);
        if (producto is null)
            return Result.Fail($"Producto {request.Id} no encontrado.");

        producto.Nombre = request.Nombre;
        producto.Descripcion = request.Descripcion;
        producto.Precio = request.Precio;
        producto.Stock = request.Stock;
        producto.EsUnico = request.EsUnico;
        producto.UpdatedAt = DateTime.UtcNow;
        producto.UpdatedBy = "system";

        if (request.NuevaImagenData is not null && request.NuevaImagenContentType is not null)
        {
            // Reemplazar imagen principal
            var imagenAnterior = producto.Imagenes.FirstOrDefault(i => i.EsPrincipal);
            if (imagenAnterior is not null)
                producto.Imagenes.Remove(imagenAnterior);

            producto.Imagenes.Add(new Domain.Entities.ProductImage
            {
                Id = Guid.NewGuid(),
                ProductoId = producto.Id,
                ImageData = request.NuevaImagenData,
                ContentType = request.NuevaImagenContentType,
                EsPrincipal = true
            });
        }

        uow.Productos.Update(producto);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok("Producto actualizado correctamente.");
    }
}
