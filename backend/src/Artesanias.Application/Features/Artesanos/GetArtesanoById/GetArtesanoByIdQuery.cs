using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Artesanos.GetArtesanoById;

public record GetArtesanoByIdQuery(Guid Id) : IRequest<Result<ArtesanoDto>>;

public class GetArtesanoByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetArtesanoByIdQuery, Result<ArtesanoDto>>
{
    public async Task<Result<ArtesanoDto>> Handle(
        GetArtesanoByIdQuery request,
        CancellationToken cancellationToken)
    {
        var artesano = await uow.Artesanos.GetByIdAsync(request.Id, cancellationToken);
        if (artesano is null)
            return Result<ArtesanoDto>.Fail($"Artesano {request.Id} no encontrado.");

        return Result<ArtesanoDto>.Ok(new ArtesanoDto
        {
            Id = artesano.Id,
            Nombre = artesano.Nombre,
            HistoriaBiografia = artesano.HistoriaBiografia,
            ComunidadOrigen = artesano.ComunidadOrigen,
            CreatedAt = artesano.CreatedAt
        });
    }
}
