namespace Artesanias.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public decimal Total { get; set; }
    public string EstadoPedido { get; set; } = string.Empty;
    public string DireccionEnvio { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
    public PaymentTransactionDto? Pago { get; set; }
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class PaymentTransactionDto
{
    public Guid Id { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string EstadoPago { get; set; } = string.Empty;
    public string StripeSessionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateOrderResponseDto
{
    public Guid OrderId { get; set; }
    public string StripeCheckoutUrl { get; set; } = string.Empty;
    public string StripeSessionId { get; set; } = string.Empty;
}
