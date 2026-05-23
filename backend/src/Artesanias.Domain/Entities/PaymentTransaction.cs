namespace Artesanias.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public string MetodoPago { get; set; } = "Stripe";
    public string EstadoPago { get; set; } = EstadosPago.Pendiente;
    public string ReferenciaPasarela { get; set; } = string.Empty;
    public string PayloadPasarela { get; set; } = string.Empty;
    public string StripeSessionId { get; set; } = string.Empty;
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Order Order { get; set; } = null!;
}

public static class EstadosPago
{
    public const string Pendiente = "Pendiente";
    public const string Pagado = "Pagado";
    public const string Fallido = "Fallido";
    public const string Reembolsado = "Reembolsado";
}
