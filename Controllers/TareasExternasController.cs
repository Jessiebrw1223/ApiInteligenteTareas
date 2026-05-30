using ApiInteligenteTareas.DTOs;
using ApiInteligenteTareas.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiInteligenteTareas.Controllers;

[ApiController]
[Route("api/tareas-externas")]
public class TareasExternasController : ControllerBase
{
    private readonly ITareaExternaService _service;

    public TareasExternasController(ITareaExternaService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TareaExternaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<TareaExternaDto>>> Get(CancellationToken cancellationToken)
    {
        try
        {
            var tareas = await _service.ObtenerTodasAsync(cancellationToken);
            return Ok(tareas);
        }
        catch (ExternalApiException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { mensaje = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TareaExternaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<TareaExternaDto>> GetPorId(int id, CancellationToken cancellationToken)
    {
        try
        {
            var tarea = await _service.ObtenerPorIdAsync(id, cancellationToken);
            return tarea is null ? NotFound(new { mensaje = "La tarea externa no existe." }) : Ok(tarea);
        }
        catch (ExternalApiException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { mensaje = ex.Message });
        }
    }
}
