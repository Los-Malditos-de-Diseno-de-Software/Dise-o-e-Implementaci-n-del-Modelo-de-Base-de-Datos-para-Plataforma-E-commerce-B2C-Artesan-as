namespace Artesanias.Application.DTOs;

public class ArtesanoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string HistoriaBiografia { get; set; } = string.Empty;
    public string ComunidadOrigen { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
