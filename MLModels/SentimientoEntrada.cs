using Microsoft.ML.Data;

namespace ApiInteligenteTareas.MLModels;

public class SentimientoEntrada
{
    [LoadColumn(0)]
    public string Comentario { get; set; } = string.Empty;

    [LoadColumn(1)]
    public bool Label { get; set; }
}
