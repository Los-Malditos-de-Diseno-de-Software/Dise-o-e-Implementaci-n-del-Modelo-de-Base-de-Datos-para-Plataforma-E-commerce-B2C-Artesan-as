namespace Artesanias.Domain.Entities;

public class Order : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public decimal Total { get; set; }
    public string EstadoPedido { get; set; } = EstadosPedido.Pendiente;
    public string DireccionEnvio { get; set; } = string.Empty;

    // Navigation
    public Usuario Usuario { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public PaymentTransaction? Pago { get; set; }
}

public static class EstadosPedido
{
    public const string Pendiente = "Pendiente";
    public const string Pagado = "Pagado";
    public const string Enviado = "Enviado";
    public const string Entregado = "Entregado";
    public const string Cancelado = "Cancelado";
}
