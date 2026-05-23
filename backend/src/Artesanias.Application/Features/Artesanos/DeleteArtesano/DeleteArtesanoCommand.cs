using Artesanias.Application.Common;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Artesanos.DeleteArtesano;

public record DeleteArtesanoCommand(Guid Id) : IRequest<Result>;

public class DeleteArtesanoCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteArtesanoCommand, Result>
{
    public async Task<Result> Handle(
        DeleteArtesanoCommand request,
        CancellationToken cancellationToken)
    {
        var artesano = await uow.Artesanos.GetByIdAsync(request.Id, cancellationToken);
        if (artesano is null)
            return Result.Fail($"Artesano {request.Id} no encontrado.");

        artesano.IsDeleted = true;
        artesano.UpdatedAt = DateTime.UtcNow;
        uow.Artesanos.Update(artesano);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok("Artesano eliminado correctamente.");
    }
}
