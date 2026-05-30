using ApiInteligenteTareas.MLModels;
using Microsoft.ML;

namespace ApiInteligenteTareas.Services;

public class SentimientoService : ISentimientoService
{
    private readonly Lazy<PredictionEngine<SentimientoEntrada, SentimientoPrediccion>> _predictionEngine;

    public SentimientoService()
    {
        _predictionEngine = new Lazy<PredictionEngine<SentimientoEntrada, SentimientoPrediccion>>(CrearMotorPrediccion);
    }

    public string Analizar(string comentario)
    {
        var prediccion = _predictionEngine.Value.Predict(new SentimientoEntrada { Comentario = comentario });
        return prediccion.PredictedLabel ? "Positivo" : "Negativo";
    }

    private static PredictionEngine<SentimientoEntrada, SentimientoPrediccion> CrearMotorPrediccion()
    {
        var mlContext = new MLContext(seed: 1);
        var datos = mlContext.Data.LoadFromEnumerable(Dataset());

        var pipeline = mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "Features",
                inputColumnName: nameof(SentimientoEntrada.Comentario))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(SentimientoEntrada.Label),
                featureColumnName: "Features"));

        var modelo = pipeline.Fit(datos);
        return mlContext.Model.CreatePredictionEngine<SentimientoEntrada, SentimientoPrediccion>(modelo);
    }

    private static IEnumerable<SentimientoEntrada> Dataset()
    {
        return new List<SentimientoEntrada>
        {
            new() { Comentario = "La tarea fue completada correctamente", Label = true },
            new() { Comentario = "El sistema funciona bien", Label = true },
            new() { Comentario = "Excelente trabajo del equipo", Label = true },
            new() { Comentario = "La respuesta fue rápida y satisfactoria", Label = true },
            new() { Comentario = "Todo se resolvió sin problemas", Label = true },
            new() { Comentario = "La implementación fue exitosa", Label = true },
            new() { Comentario = "El servicio está estable y correcto", Label = true },
            new() { Comentario = "La tarea se entregó a tiempo", Label = true },

            new() { Comentario = "La tarea falló y generó errores", Label = false },
            new() { Comentario = "El sistema no funciona", Label = false },
            new() { Comentario = "Hubo un problema grave en producción", Label = false },
            new() { Comentario = "La respuesta fue lenta y deficiente", Label = false },
            new() { Comentario = "No se pudo completar la tarea", Label = false },
            new() { Comentario = "El servicio está caído", Label = false },
            new() { Comentario = "La implementación salió mal", Label = false },
            new() { Comentario = "Existen errores críticos pendientes", Label = false }
        };
    }
}
