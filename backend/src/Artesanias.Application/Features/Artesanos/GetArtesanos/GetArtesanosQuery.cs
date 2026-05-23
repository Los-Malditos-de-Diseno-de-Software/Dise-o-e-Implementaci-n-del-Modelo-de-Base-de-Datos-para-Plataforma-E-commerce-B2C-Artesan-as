using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Artesanos.GetArtesanos;

public record GetArtesanosQuery : IRequest<Result<List<ArtesanoDto>>>;

public class GetArtesanosQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetArtesanosQuery, Result<List<ArtesanoDto>>>
{
    public async Task<Result<List<ArtesanoDto>>> Handle(
        GetArtesanosQuery request,
        CancellationToken cancellationToken)
    {
        var artesanos = await uow.Artesanos.GetAllAsync(cancellationToken);

        var dtos = artesanos.Select(a => new ArtesanoDto
        {
            Id = a.Id,
            Nombre = a.Nombre,
            HistoriaBiografia = a.HistoriaBiografia,
            ComunidadOrigen = a.ComunidadOrigen,
            CreatedAt = a.CreatedAt
        }).ToList();

        return Result<List<ArtesanoDto>>.Ok(dtos);
    }
}
