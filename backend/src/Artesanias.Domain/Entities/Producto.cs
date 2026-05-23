namespace Artesanias.Domain.Entities;

public class Producto : BaseEntity
{
    public Guid ArtesanoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool EsUnico { get; set; }

    // Navigation
    public Artesano Artesano { get; set; } = null!;
    public ICollection<ProductImage> Imagenes { get; set; } = new List<ProductImage>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
