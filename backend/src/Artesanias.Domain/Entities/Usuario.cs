namespace Artesanias.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = Roles.Cliente;
    public string Telefono { get; set; } = string.Empty;

    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<ShoppingCart> Carritos { get; set; } = new List<ShoppingCart>();
}

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Cliente = "Cliente";
}
