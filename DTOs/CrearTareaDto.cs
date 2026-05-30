using System.ComponentModel.DataAnnotations;

namespace ApiInteligenteTareas.DTOs;

public class CrearTareaDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    public string Prioridad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
    public DateTime FechaVencimiento { get; set; }
}
