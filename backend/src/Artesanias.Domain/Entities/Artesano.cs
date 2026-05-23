namespace Artesanias.Domain.Entities;

public class Artesano : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string HistoriaBiografia { get; set; } = string.Empty;
    public string ComunidadOrigen { get; set; } = string.Empty;

    // Navigation
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
