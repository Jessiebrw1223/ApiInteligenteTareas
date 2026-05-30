using ApiInteligenteTareas.Data;
using ApiInteligenteTareas.DTOs;
using ApiInteligenteTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiInteligenteTareas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TareasController : ControllerBase
{
    private readonly AppDbContext _context;

    public TareasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TareaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<TareaDto>>> GetTareas(
        [FromQuery] string? estado,
        [FromQuery] string? prioridad,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin)
    {
        if (!string.IsNullOrWhiteSpace(estado) && !EstadosTarea.EsValido(estado))
            return BadRequest(new { mensaje = "El estado no es válido. Valores permitidos: Pendiente, EnProceso, Completada." });

        if (!string.IsNullOrWhiteSpace(prioridad) && !PrioridadesTarea.EsValida(prioridad))
            return BadRequest(new { mensaje = "La prioridad no es válida. Valores permitidos: Baja, Media, Alta." });

        if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio.Value.Date > fechaFin.Value.Date)
            return BadRequest(new { mensaje = "La fechaInicio no puede ser mayor que fechaFin." });

        IQueryable<Tarea> query = _context.Tareas.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(estado))
        {
            var estadoNormalizado = EstadosTarea.Normalizar(estado);
            query = query.Where(t => t.Estado == estadoNormalizado);
        }

        if (!string.IsNullOrWhiteSpace(prioridad))
        {
            var prioridadNormalizada = PrioridadesTarea.Normalizar(prioridad);
            query = query.Where(t => t.Prioridad == prioridadNormalizada);
        }

        if (fechaInicio.HasValue)
            query = query.Where(t => t.FechaVencimiento.Date >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(t => t.FechaVencimiento.Date <= fechaFin.Value.Date);

        var tareas = await query
            .OrderBy(t => t.FechaVencimiento)
            .Select(t => MapearDto(t))
            .ToListAsync();

        return Ok(tareas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TareaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TareaDto>> GetTarea(int id)
    {
        var tarea = await _context.Tareas.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return tarea is null ? NotFound(new { mensaje = "Tarea no encontrada." }) : Ok(MapearDto(tarea));
    }

    [HttpPost]
    [ProducesResponseType(typeof(TareaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TareaDto>> CrearTarea([FromBody] CrearTareaDto dto)
    {
        var error = ValidarDatosTarea(dto.Titulo, dto.Estado, dto.Prioridad, dto.FechaVencimiento);
        if (error is not null) return BadRequest(new { mensaje = error });

        var tarea = new Tarea
        {
            Titulo = dto.Titulo.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            Estado = EstadosTarea.Normalizar(dto.Estado),
            Prioridad = PrioridadesTarea.Normalizar(dto.Prioridad),
            FechaCreacion = DateTime.UtcNow,
            FechaVencimiento = dto.FechaVencimiento.Date
        };

        _context.Tareas.Add(tarea);
        await _context.SaveChangesAsync();

        var respuesta = MapearDto(tarea);
        return CreatedAtAction(nameof(GetTarea), new { id = tarea.Id }, respuesta);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarTarea(int id, [FromBody] ActualizarTareaDto dto)
    {
        var tarea = await _context.Tareas.FindAsync(id);
        if (tarea is null) return NotFound(new { mensaje = "Tarea no encontrada." });

        var error = ValidarDatosTarea(dto.Titulo, dto.Estado, dto.Prioridad, dto.FechaVencimiento);
        if (error is not null) return BadRequest(new { mensaje = error });

        tarea.Titulo = dto.Titulo.Trim();
        tarea.Descripcion = dto.Descripcion?.Trim();
        tarea.Estado = EstadosTarea.Normalizar(dto.Estado);
        tarea.Prioridad = PrioridadesTarea.Normalizar(dto.Prioridad);
        tarea.FechaVencimiento = dto.FechaVencimiento.Date;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarTarea(int id)
    {
        var tarea = await _context.Tareas.FindAsync(id);
        if (tarea is null) return NotFound(new { mensaje = "Tarea no encontrada." });

        _context.Tareas.Remove(tarea);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static string? ValidarDatosTarea(string titulo, string estado, string prioridad, DateTime fechaVencimiento)
    {
        if (string.IsNullOrWhiteSpace(titulo)) return "El título es obligatorio.";
        if (!EstadosTarea.EsValido(estado)) return "El estado no es válido. Valores permitidos: Pendiente, EnProceso, Completada.";
        if (!PrioridadesTarea.EsValida(prioridad)) return "La prioridad no es válida. Valores permitidos: Baja, Media, Alta.";
        if (fechaVencimiento.Date < DateTime.UtcNow.Date) return "La fecha de vencimiento no puede ser menor a la fecha actual.";
        return null;
    }

    private static TareaDto MapearDto(Tarea tarea) => new()
    {
        Id = tarea.Id,
        Titulo = tarea.Titulo,
        Descripcion = tarea.Descripcion,
        Estado = tarea.Estado,
        Prioridad = tarea.Prioridad,
        FechaCreacion = tarea.FechaCreacion,
        FechaVencimiento = tarea.FechaVencimiento
    };
}
