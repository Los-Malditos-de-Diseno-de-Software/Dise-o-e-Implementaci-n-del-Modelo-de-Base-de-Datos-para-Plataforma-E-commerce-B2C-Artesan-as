using Artesanias.Application.Common;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Productos.DeleteProducto;

public record DeleteProductoCommand(Guid Id) : IRequest<Result>;

public class DeleteProductoCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteProductoCommand, Result>
{
    public async Task<Result> Handle(
        DeleteProductoCommand request,
        CancellationToken cancellationToken)
    {
        var producto = await uow.Productos.GetByIdAsync(request.Id, cancellationToken);
        if (producto is null)
            return Result.Fail($"Producto {request.Id} no encontrado.");

        // Soft delete — el filtro global de EF Core lo excluye automáticamente
        producto.IsDeleted = true;
        producto.UpdatedAt = DateTime.UtcNow;
        uow.Productos.Update(producto);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok("Producto eliminado correctamente.");
    }
}
