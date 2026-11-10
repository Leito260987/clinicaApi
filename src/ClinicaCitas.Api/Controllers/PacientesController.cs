using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.DTOs.Common;
using ClinicaCitas.Api.DTOs.Pacientes;
using ClinicaCitas.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaCitas.Api.Controllers;

/// <summary>Gestión de pacientes.</summary>
[ApiController]
[Route("api/pacientes")]
[Produces("application/json")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _pacientes;
    private readonly ICitaService _citas;

    public PacientesController(IPacienteService pacientes, ICitaService citas)
    {
        _pacientes = pacientes;
        _citas = citas;
    }

    /// <summary>Lista los pacientes. Permite buscar por nombre o DNI.</summary>
    /// <param name="busqueda">Texto a buscar en el nombre completo o en el DNI (opcional).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PacienteResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string? busqueda)
    {
        var datos = await _pacientes.ListarAsync(busqueda);
        return Ok(ApiResponse<IReadOnlyList<PacienteResponseDto>>.Ok(datos, $"Se encontraron {datos.Count} paciente(s)."));
    }

    /// <summary>Obtiene un paciente por su id.</summary>
    [HttpGet("{id:int}", Name = nameof(ObtenerPaciente))]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPaciente(int id) =>
        Ok(ApiResponse<PacienteResponseDto>.Ok(await _pacientes.ObtenerAsync(id), "Paciente encontrado."));

    /// <summary>Registra un nuevo paciente.</summary>
    /// <response code="201">Paciente registrado; la cabecera Location apunta al nuevo recurso.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    /// <response code="409">Ya existe un paciente con ese DNI.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear([FromBody] PacienteRequestDto dto)
    {
        var creado = await _pacientes.CrearAsync(dto);
        return CreatedAtRoute(nameof(ObtenerPaciente), new { id = creado.Id },
            ApiResponse<PacienteResponseDto>.Ok(creado, "Paciente registrado correctamente."));
    }

    /// <summary>Actualiza todos los datos de un paciente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] PacienteRequestDto dto) =>
        Ok(ApiResponse<PacienteResponseDto>.Ok(await _pacientes.ActualizarAsync(id, dto), "Paciente actualizado correctamente."));

    /// <summary>Elimina un paciente que no tenga citas registradas.</summary>
    /// <response code="204">Paciente eliminado.</response>
    /// <response code="409">El paciente tiene citas registradas.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _pacientes.EliminarAsync(id);
        return NoContent();
    }

    /// <summary>Lista las citas de un paciente (ruta jerárquica).</summary>
    [HttpGet("{id:int}/citas")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CitaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarCitas(int id)
    {
        var datos = await _citas.ListarAsync(new CitaFiltroDto { PacienteId = id });
        return Ok(ApiResponse<IReadOnlyList<CitaResponseDto>>.Ok(datos, $"El paciente tiene {datos.Count} cita(s)."));
    }
}
