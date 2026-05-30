namespace ApiInteligenteTareas.Models;

public static class EstadosTarea
{
    public const string Pendiente = "Pendiente";
    public const string EnProceso = "EnProceso";
    public const string Completada = "Completada";

    public static readonly string[] Permitidos = [Pendiente, EnProceso, Completada];

    public static bool EsValido(string? valor) =>
        !string.IsNullOrWhiteSpace(valor) && Permitidos.Contains(valor, StringComparer.OrdinalIgnoreCase);

    public static string Normalizar(string valor) =>
        Permitidos.First(x => x.Equals(valor, StringComparison.OrdinalIgnoreCase));
}
