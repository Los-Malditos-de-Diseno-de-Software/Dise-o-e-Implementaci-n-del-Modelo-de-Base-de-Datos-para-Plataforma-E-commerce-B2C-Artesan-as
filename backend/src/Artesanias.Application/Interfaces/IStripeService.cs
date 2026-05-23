using Artesanias.Application.Features.Pedidos.CreateOrder;

namespace Artesanias.Application.Interfaces;

/// <summary>
/// Abstracción de Stripe definida en Application para que los handlers no dependan de Infrastructure.
/// La implementación concreta vive en Artesanias.Infrastructure/Services/StripeService.cs
/// </summary>
public interface IStripeService
{
    Task<StripeSessionResult> CreateCheckoutSessionAsync(
        Guid orderId,
        IEnumerable<StripeLineItem> lineItems,
        string successUrl,
        string cancelUrl,
        CancellationToken ct = default);
}
