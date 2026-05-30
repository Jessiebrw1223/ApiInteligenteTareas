using System.ComponentModel.DataAnnotations;

namespace ApiInteligenteTareas.Models;

public class Tarea
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [Required]
    [MaxLength(20)]
    public string Estado { get; set; } = EstadosTarea.Pendiente;

    [Required]
    [MaxLength(20)]
    public string Prioridad { get; set; } = PrioridadesTarea.Media;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime FechaVencimiento { get; set; }
}
