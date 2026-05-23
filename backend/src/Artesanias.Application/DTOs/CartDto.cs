namespace Artesanias.Application.DTOs;

public class CartDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
    public decimal Total => Items.Sum(i => i.Subtotal);
    public int TotalItems => Items.Sum(i => i.Cantidad);
}

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public string? ProductoImagenBase64 { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitarioCongelado { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitarioCongelado;
}
