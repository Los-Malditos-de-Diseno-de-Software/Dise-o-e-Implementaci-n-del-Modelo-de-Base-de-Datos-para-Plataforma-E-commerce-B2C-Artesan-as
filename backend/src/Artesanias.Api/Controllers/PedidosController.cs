using Artesanias.Application.Features.Pedidos.CreateOrder;
using Artesanias.Application.Features.Pedidos.GetOrderById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Artesanias.Api.Controllers;

public class PedidosController : BaseApiController
{
    private readonly IConfiguration _configuration;

    public PedidosController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    // [Authorize] // Descomentar en Fase 4
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // En Fase 4, este Guid vendrá del token JWT (User.Claims)
        // Por ahora lo tomamos del request (simulación)
        var usuarioId = request.UsuarioId;

        // Leer X-Session-Id
        if (!Request.Headers.TryGetValue("X-Session-Id", out var headerValue) || !Guid.TryParse(headerValue, out var sessionId))
        {
            return BadRequest("Falta el header X-Session-Id o su formato es inválido.");
        }

        var successUrl = _configuration["Stripe:SuccessUrl"] ?? "http://localhost:5173/confirmacion?session_id={CHECKOUT_SESSION_ID}";
        var cancelUrl = _configuration["Stripe:CancelUrl"] ?? "http://localhost:5173/carrito";

        var command = new CreateOrderCommand(
            sessionId,
            usuarioId,
            request.DireccionEnvio,
            successUrl,
            cancelUrl
        );

        return HandleResult(await Mediator.Send(command));
    }

    [HttpGet("{id}")]
    // [Authorize] // Descomentar en Fase 4
    public async Task<IActionResult> GetOrderById(Guid id, [FromQuery] Guid usuarioId)
    {
        // El usuarioId vendrá del token en el futuro, por ahora via Query param para simular
        return HandleResult(await Mediator.Send(new GetOrderByIdQuery(id, usuarioId)));
    }

    [HttpGet]
    // [Authorize(Roles = "Administrador")] // Descomentar en Fase 4
    public async Task<IActionResult> GetAllOrders()
    {
        return HandleResult(await Mediator.Send(new Artesanias.Application.Features.Pedidos.GetAllOrders.GetAllOrdersQuery()));
    }
}

public record CreateOrderRequest(Guid UsuarioId, string DireccionEnvio);
