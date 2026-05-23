using Artesanias.Application.Features.Productos.CreateProducto;
using Artesanias.Application.Features.Productos.DeleteProducto;
using Artesanias.Application.Features.Productos.GetProductoById;
using Artesanias.Application.Features.Productos.GetProductos;
using Artesanias.Application.Features.Productos.UpdateProducto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artesanias.Api.Controllers;

public class ProductosController : BaseApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductos([FromQuery] int page = 1, [FromQuery] int pageSize = 12, [FromQuery] string? search = null)
    {
        return HandleResult(await Mediator.Send(new GetProductosQuery(page, pageSize, search)));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductoById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetProductoByIdQuery(id)));
    }

    [HttpPost]
    // [Authorize(Roles = "Administrador")] // Descomentar en Fase 4
    public async Task<IActionResult> CreateProducto([FromForm] CreateProductoRequest request)
    {
        byte[]? imagenData = null;
        string? contentType = null;

        if (request.Imagen is not null && request.Imagen.Length > 0)
        {
            using var ms = new MemoryStream();
            await request.Imagen.CopyToAsync(ms);
            imagenData = ms.ToArray();
            contentType = request.Imagen.ContentType;
        }

        var command = new CreateProductoCommand(
            request.ArtesanoId,
            request.Nombre,
            request.Descripcion,
            request.Precio,
            request.Stock,
            request.EsUnico,
            imagenData,
            contentType
        );

        return HandleResult(await Mediator.Send(command));
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Administrador")] // Descomentar en Fase 4
    public async Task<IActionResult> UpdateProducto(Guid id, [FromForm] UpdateProductoRequest request)
    {
        byte[]? imagenData = null;
        string? contentType = null;

        if (request.NuevaImagen is not null && request.NuevaImagen.Length > 0)
        {
            using var ms = new MemoryStream();
            await request.NuevaImagen.CopyToAsync(ms);
            imagenData = ms.ToArray();
            contentType = request.NuevaImagen.ContentType;
        }

        var command = new UpdateProductoCommand(
            id,
            request.Nombre,
            request.Descripcion,
            request.Precio,
            request.Stock,
            request.EsUnico,
            imagenData,
            contentType
        );

        return HandleResult(await Mediator.Send(command));
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Administrador")] // Descomentar en Fase 4
    public async Task<IActionResult> DeleteProducto(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteProductoCommand(id)));
    }
}

public record CreateProductoRequest(
    Guid ArtesanoId,
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    bool EsUnico,
    IFormFile? Imagen
);

public record UpdateProductoRequest(
    string Nombre,
    string Descripcion,
    decimal Precio,
    int Stock,
    bool EsUnico,
    IFormFile? NuevaImagen
);
