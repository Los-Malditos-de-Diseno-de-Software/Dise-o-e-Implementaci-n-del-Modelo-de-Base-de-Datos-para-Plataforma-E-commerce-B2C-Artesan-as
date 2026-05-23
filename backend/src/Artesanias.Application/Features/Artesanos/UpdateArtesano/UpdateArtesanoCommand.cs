using Artesanias.Application.Common;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Artesanos.UpdateArtesano;

public record UpdateArtesanoCommand(
    Guid Id,
    string Nombre,
    string HistoriaBiografia,
    string ComunidadOrigen
) : IRequest<Result>;

public class UpdateArtesanoCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateArtesanoCommand, Result>
{
    public async Task<Result> Handle(
        UpdateArtesanoCommand request,
        CancellationToken cancellationToken)
    {
        var artesano = await uow.Artesanos.GetByIdAsync(request.Id, cancellationToken);
        if (artesano is null)
            return Result.Fail($"Artesano {request.Id} no encontrado.");

        artesano.Nombre = request.Nombre;
        artesano.HistoriaBiografia = request.HistoriaBiografia;
        artesano.ComunidadOrigen = request.ComunidadOrigen;
        artesano.UpdatedAt = DateTime.UtcNow;
        artesano.UpdatedBy = "system";

        uow.Artesanos.Update(artesano);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok("Artesano actualizado correctamente.");
    }
}
