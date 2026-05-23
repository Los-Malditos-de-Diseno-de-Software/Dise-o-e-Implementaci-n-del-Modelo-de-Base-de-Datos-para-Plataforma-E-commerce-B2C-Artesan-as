namespace Artesanias.Domain.Entities;

public class CartItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShoppingCartId { get; set; }
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitarioCongelado { get; set; }

    // Navigation
    public ShoppingCart ShoppingCart { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
