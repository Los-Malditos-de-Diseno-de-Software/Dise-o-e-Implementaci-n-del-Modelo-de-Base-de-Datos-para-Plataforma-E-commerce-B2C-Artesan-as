using Artesanias.Application.Common;
using Artesanias.Application.DTOs;
using Artesanias.Domain.Interfaces;
using MediatR;

namespace Artesanias.Application.Features.Productos.GetProductos;

public record GetProductosQuery(int Page = 1, int PageSize = 12, string? Search = null)
    : IRequest<Result<PagedResult<ProductoResumenDto>>>;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class GetProductosQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetProductosQuery, Result<PagedResult<ProductoResumenDto>>>
{
    public async Task<Result<PagedResult<ProductoResumenDto>>> Handle(
        GetProductosQuery request,
        CancellationToken cancellationToken)
    {
        var (productos, total) = await uow.Productos.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            cancellationToken);

        var dtos = productos.Select(p =>
        {
            var imagenPrincipal = p.Imagenes.FirstOrDefault(i => i.EsPrincipal)
                                  ?? p.Imagenes.FirstOrDefault();

            string? base64 = null;
            if (imagenPrincipal is not null)
                base64 = $"data:{imagenPrincipal.ContentType};base64,{Convert.ToBase64String(imagenPrincipal.ImageData)}";

            return new ProductoResumenDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Stock = p.Stock,
                EsUnico = p.EsUnico,
                ArtesanoNombre = p.Artesano?.Nombre ?? string.Empty,
                ImagenBase64 = base64
            };
        }).ToList();

        return Result<PagedResult<ProductoResumenDto>>.Ok(new PagedResult<ProductoResumenDto>
        {
            Items = dtos,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}
