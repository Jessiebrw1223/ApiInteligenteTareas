namespace ApiInteligenteTareas.DTOs;

public class TareaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaVencimiento { get; set; }
}
