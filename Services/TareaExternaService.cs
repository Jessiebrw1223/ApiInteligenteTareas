using System.Net;
using System.Text.Json.Serialization;
using ApiInteligenteTareas.DTOs;

namespace ApiInteligenteTareas.Services;

public class TareaExternaService : ITareaExternaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TareaExternaService> _logger;

    public TareaExternaService(HttpClient httpClient, ILogger<TareaExternaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TareaExternaDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var todos = await _httpClient.GetFromJsonAsync<List<JsonPlaceholderTodo>>("todos", cancellationToken);
            return todos?.Select(Mapear).ToList() ?? [];
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error al consumir la API externa de tareas.");
            throw new ExternalApiException("No se pudo obtener información desde la API externa.", ex);
        }
    }

    public async Task<TareaExternaDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"todos/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            var todo = await response.Content.ReadFromJsonAsync<JsonPlaceholderTodo>(cancellationToken: cancellationToken);
            return todo is null ? null : Mapear(todo);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error al consumir la API externa de tareas por ID {Id}.", id);
            throw new ExternalApiException("No se pudo obtener información desde la API externa.", ex);
        }
    }

    private static TareaExternaDto Mapear(JsonPlaceholderTodo todo) => new()
    {
        ExternalId = todo.Id,
        Titulo = todo.Title,
        Completado = todo.Completed
    };

    private sealed class JsonPlaceholderTodo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
    }
}
