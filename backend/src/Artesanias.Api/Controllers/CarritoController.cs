using Artesanias.Application.Features.Cart.AddCartItem;
using Artesanias.Application.Features.Cart.GetCart;
using Artesanias.Application.Features.Cart.RemoveCartItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artesanias.Api.Controllers;

[AllowAnonymous] // El carrito puede ser accedido por usuarios anónimos a través de X-Session-Id
public class CarritoController : BaseApiController
{
    private Guid GetSessionId()
    {
        if (Request.Headers.TryGetValue("X-Session-Id", out var headerValue) && Guid.TryParse(headerValue, out var sessionId))
        {
            return sessionId;
        }
        
        throw new InvalidOperationException("Falta el header X-Session-Id o su formato es inválido.");
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var sessionId = GetSessionId();
        return HandleResult(await Mediator.Send(new GetCartQuery(sessionId)));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddCartItem([FromBody] AddCartItemRequest request)
    {
        var sessionId = GetSessionId();
        var command = new AddCartItemCommand(sessionId, request.ProductoId, request.Cantidad);
        return HandleResult(await Mediator.Send(command));
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
    {
        var sessionId = GetSessionId();
        var command = new RemoveCartItemCommand(sessionId, cartItemId);
        return HandleResult(await Mediator.Send(command));
    }
}

public record AddCartItemRequest(Guid ProductoId, int Cantidad);
