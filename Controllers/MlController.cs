using ApiInteligenteTareas.DTOs;
using ApiInteligenteTareas.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiInteligenteTareas.Controllers;

[ApiController]
[Route("api/ml")]
public class MlController : ControllerBase
{
    private readonly ISentimientoService _sentimientoService;

    public MlController(ISentimientoService sentimientoService)
    {
        _sentimientoService = sentimientoService;
    }

    [HttpPost("sentimiento")]
    [ProducesResponseType(typeof(SentimientoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<SentimientoResponse> AnalizarSentimiento([FromBody] SentimientoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Comentario))
            return BadRequest(new { mensaje = "El comentario es obligatorio." });

        var sentimiento = _sentimientoService.Analizar(request.Comentario);
        return Ok(new SentimientoResponse
        {
            Comentario = request.Comentario,
            Sentimiento = sentimiento
        });
    }
}
