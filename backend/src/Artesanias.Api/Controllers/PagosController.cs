using Artesanias.Application.Features.Pedidos.ConfirmPayment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Artesanias.Api.Controllers;

public class PagosController : BaseApiController
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PagosController> _logger;

    public PagosController(IConfiguration configuration, ILogger<PagosController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("webhook")]
    [AllowAnonymous] // Stripe llama a este endpoint sin token JWT
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"];
        var webhookSecret = _configuration["Stripe:WebhookSecret"];

        if (string.IsNullOrEmpty(webhookSecret))
        {
            _logger.LogError("Stripe:WebhookSecret no está configurado.");
            return StatusCode(500, "Webhook secret not configured");
        }

        try
        {
            // Valida la firma del payload con el secreto de tu cuenta
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signatureHeader,
                webhookSecret
            );

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                if (session is not null)
                {
                    _logger.LogInformation("Recibido checkout.session.completed para SessionId: {SessionId}", session.Id);

                    var command = new ConfirmPaymentCommand(
                        StripeSessionId: session.Id,
                        StripePaymentIntentId: session.PaymentIntentId ?? string.Empty,
                        RawPayload: json
                    );

                    var result = await Mediator.Send(command);
                    if (!result.Success)
                    {
                        _logger.LogError("Error confirmando pago: {Error}", string.Join(", ", result.Errors));
                        return BadRequest(result);
                    }
                    
                    _logger.LogInformation("Pago confirmado correctamente y stock descontado.");
                }
            }

            return Ok(); // Es importante responder 200 OK rápido a Stripe
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Error validando firma de Stripe");
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error procesando webhook de Stripe");
            return StatusCode(500);
        }
    }
}
