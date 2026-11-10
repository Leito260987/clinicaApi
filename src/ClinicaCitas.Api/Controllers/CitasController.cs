using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.DTOs.Common;
using ClinicaCitas.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaCitas.Api.Controllers;

/// <summary>Registro, consulta, reprogramación, cancelación y eliminación de citas médicas.</summary>
[ApiController]
[Route("api/citas")]
[Produces("application/json")]
public class CitasController : ControllerBase
{
    private readonly ICitaService _citas;

    public CitasController(ICitaService citas) => _citas = citas;

    /// <summary>Lista las citas, con filtros opcionales por paciente, médico, estado y fecha.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CitaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Listar([FromQuery] CitaFiltroDto filtro)
    {
        var datos = await _citas.ListarAsync(filtro);
        return Ok(ApiResponse<IReadOnlyList<CitaResponseDto>>.Ok(datos, $"Se encontraron {datos.Count} cita(s)."));
    }

    /// <summary>Obtiene una cita por su id.</summary>
    [HttpGet("{id:int}", Name = nameof(ObtenerCita))]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerCita(int id) =>
        Ok(ApiResponse<CitaResponseDto>.Ok(await _citas.ObtenerAsync(id), "Cita encontrada."));

    /// <summary>Registra una cita médica.</summary>
    /// <remarks>
    /// Reglas: lunes a sábado de 08:00 a 20:00, bloques de 30 minutos, fecha futura,
    /// médico activo y sin cruces de horario para el médico ni para el paciente.
    /// </remarks>
    /// <response code="201">Cita registrada en estado Programada.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    /// <response code="404">El paciente o el médico no existen.</response>
    /// <response code="409">El médico o el paciente ya tienen una cita en ese horario.</response>
    /// <response code="422">Fuera del horario de atención o médico inactivo.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Crear([FromBody] CitaCrearDto dto)
    {
        var creada = await _citas.CrearAsync(dto);
        return CreatedAtRoute(nameof(ObtenerCita), new { id = creada.Id },
            ApiResponse<CitaResponseDto>.Ok(creada, "Cita registrada correctamente."));
    }

    /// <summary>Reprograma una cita (médico, fecha y motivo). Solo citas en estado Programada.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Reprogramar(int id, [FromBody] CitaActualizarDto dto) =>
        Ok(ApiResponse<CitaResponseDto>.Ok(await _citas.ReprogramarAsync(id, dto), "Cita reprogramada correctamente."));

    /// <summary>Cancela una cita y libera el horario.</summary>
    [HttpPatch("{id:int}/cancelar")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cancelar(int id, [FromBody] CitaCancelarDto dto) =>
        Ok(ApiResponse<CitaResponseDto>.Ok(await _citas.CancelarAsync(id, dto), "Cita cancelada correctamente."));

    /// <summary>Marca una cita como atendida.</summary>
    [HttpPatch("{id:int}/atender")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Atender(int id) =>
        Ok(ApiResponse<CitaResponseDto>.Ok(await _citas.MarcarAtendidaAsync(id), "Cita marcada como atendida."));

    /// <summary>Elimina una cita. Las citas atendidas no se eliminan (historial).</summary>
    /// <response code="204">Cita eliminada.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _citas.EliminarAsync(id);
        return NoContent();
    }
}
