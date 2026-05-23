using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Artesanos.CreateArtesano;

public record CreateArtesanoCommand(
    string Nombre,
    string HistoriaBiografia,
    string ComunidadOrigen
) : IRequest<Result<ArtesanoDto>>;

public class CreateArtesanoCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateArtesanoCommand, Result<ArtesanoDto>>
{
    public async Task<Result<ArtesanoDto>> Handle(
        CreateArtesanoCommand request,
        CancellationToken cancellationToken)
    {
        var artesano = new Artesano
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            HistoriaBiografia = request.HistoriaBiografia,
            ComunidadOrigen = request.ComunidadOrigen,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "system",
            UpdatedBy = "system"
        };

        await uow.Artesanos.AddAsync(artesano, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<ArtesanoDto>.Ok(new ArtesanoDto
        {
            Id = artesano.Id,
            Nombre = artesano.Nombre,
            HistoriaBiografia = artesano.HistoriaBiografia,
            ComunidadOrigen = artesano.ComunidadOrigen,
            CreatedAt = artesano.CreatedAt
        }, "Artesano creado correctamente.");
    }
}
