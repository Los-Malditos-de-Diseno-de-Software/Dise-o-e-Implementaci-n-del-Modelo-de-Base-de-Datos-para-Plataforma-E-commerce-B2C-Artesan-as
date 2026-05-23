using Artesanias.Application.Features.Artesanos.CreateArtesano;
using Artesanias.Application.Features.Artesanos.DeleteArtesano;
using Artesanias.Application.Features.Artesanos.GetArtesanoById;
using Artesanias.Application.Features.Artesanos.GetArtesanos;
using Artesanias.Application.Features.Artesanos.UpdateArtesano;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artesanias.Api.Controllers;

public class ArtesanosController : BaseApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetArtesanos()
    {
        return HandleResult(await Mediator.Send(new GetArtesanosQuery()));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArtesanoById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetArtesanoByIdQuery(id)));
    }

    [HttpPost]
    // [Authorize(Roles = "Administrador")] // Descomentar en Fase 4
    public async Task<IActionResult> CreateArtesano(CreateArtesanoCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Administrador")] // Descomentar en Fase 4
    public async Task<IActionResult> UpdateArtesano(Guid id, UpdateArtesanoCommand command)
    {
        if (id != command.Id) return BadRequest("El Id de la URL no coincide con el Id del cuerpo.");
        return HandleResult(await Mediator.Send(command));
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Administrador")] // Descomentar en Fase 4
    public async Task<IActionResult> DeleteArtesano(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteArtesanoCommand(id)));
    }
}
