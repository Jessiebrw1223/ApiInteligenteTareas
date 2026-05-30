namespace ApiInteligenteTareas.Models;

public static class PrioridadesTarea
{
    public const string Baja = "Baja";
    public const string Media = "Media";
    public const string Alta = "Alta";

    public static readonly string[] Permitidas = [Baja, Media, Alta];

    public static bool EsValida(string? valor) =>
        !string.IsNullOrWhiteSpace(valor) && Permitidas.Contains(valor, StringComparer.OrdinalIgnoreCase);

    public static string Normalizar(string valor) =>
        Permitidas.First(x => x.Equals(valor, StringComparison.OrdinalIgnoreCase));
}
