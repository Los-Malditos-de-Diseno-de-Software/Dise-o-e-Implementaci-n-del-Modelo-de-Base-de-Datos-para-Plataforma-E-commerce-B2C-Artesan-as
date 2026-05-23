namespace Artesanias.Domain.Entities;

public class ShoppingCart
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid? UsuarioId { get; set; }
    public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;

    // Navigation
    public Usuario? Usuario { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
