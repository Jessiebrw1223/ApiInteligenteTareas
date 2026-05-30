using System.ComponentModel.DataAnnotations;

namespace ApiInteligenteTareas.DTOs;

public class SentimientoRequest
{
    [Required(ErrorMessage = "El comentario es obligatorio.")]
    public string Comentario { get; set; } = string.Empty;
}
