namespace Artesanias.Application.DTOs;

public class ProductoDto
{
    public Guid Id { get; set; }
    public Guid ArtesanoId { get; set; }
    public string ArtesanoNombre { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool EsUnico { get; set; }
    public DateTime CreatedAt { get; set; }

    // Imagen principal en Base64 — formato: "data:image/jpeg;base64,..."
    public string? ImagenBase64 { get; set; }
}

public class ProductoResumenDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool EsUnico { get; set; }
    public string ArtesanoNombre { get; set; } = string.Empty;
    public string? ImagenBase64 { get; set; }
}
