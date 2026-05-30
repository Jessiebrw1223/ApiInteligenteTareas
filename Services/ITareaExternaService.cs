using ApiInteligenteTareas.DTOs;

namespace ApiInteligenteTareas.Services;

public interface ITareaExternaService
{
    Task<IReadOnlyList<TareaExternaDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<TareaExternaDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}
