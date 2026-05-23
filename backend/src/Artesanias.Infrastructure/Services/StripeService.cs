using Artesanias.Application.Features.Pedidos.CreateOrder;
using Artesanias.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace Artesanias.Infrastructure.Services;

public class StripeService : IStripeService
{
    public StripeService(IConfiguration configuration)
    {
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"]
            ?? throw new InvalidOperationException("Stripe:SecretKey no configurada.");
    }

    public async Task<StripeSessionResult> CreateCheckoutSessionAsync(
        Guid orderId,
        IEnumerable<StripeLineItem> lineItems,
        string successUrl,
        string cancelUrl,
        CancellationToken ct = default)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems = lineItems.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "pen", // Soles peruanos
                    UnitAmountDecimal = item.PrecioUnitario * 100, // Stripe usa centavos
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Nombre
                    }
                },
                Quantity = item.Cantidad
            }).ToList(),
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "order_id", orderId.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);

        return new StripeSessionResult(session.Id, session.Url);
    }
}
