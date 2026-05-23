using Artesanias.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Artesanias.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result is null) return NotFound();
        if (result.Success && result.Data is not null) return Ok(result);
        if (result.Success && result.Data is null) return NotFound(result);
        return BadRequest(result);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result is null) return NotFound();
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}
