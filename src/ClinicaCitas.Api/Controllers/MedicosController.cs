using System.ComponentModel.DataAnnotations;
using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.DTOs.Common;
using ClinicaCitas.Api.DTOs.Medicos;
using ClinicaCitas.Api.Models.Enums;
using ClinicaCitas.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaCitas.Api.Controllers;

/// <summary>Gestión de médicos y su disponibilidad.</summary>
[ApiController]
[Route("api/medicos")]
[Produces("application/json")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _medicos;
    private readonly ICitaService _citas;

    public MedicosController(IMedicoService medicos, ICitaService citas)
    {
        _medicos = medicos;
        _citas = citas;
    }

    /// <summary>Lista los médicos. Permite filtrar por especialidad.</summary>
    /// <param name="especialidad">MedicinaGeneral, Pediatria, Cardiologia, Dermatologia, Ginecologia o Traumatologia.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MedicoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] Especialidad? especialidad)
    {
        var datos = await _medicos.ListarAsync(especialidad);
        return Ok(ApiResponse<IReadOnlyList<MedicoResponseDto>>.Ok(datos, $"Se encontraron {datos.Count} médico(s)."));
    }

    /// <summary>Obtiene un médico por su id.</summary>
    [HttpGet("{id:int}", Name = nameof(ObtenerMedico))]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerMedico(int id) =>
        Ok(ApiResponse<MedicoResponseDto>.Ok(await _medicos.ObtenerAsync(id), "Médico encontrado."));

    /// <summary>Registra un nuevo médico.</summary>
    /// <response code="201">Médico registrado.</response>
    /// <response code="409">Ya existe un médico con ese CMP o DNI.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear([FromBody] MedicoRequestDto dto)
    {
        var creado = await _medicos.CrearAsync(dto);
        return CreatedAtRoute(nameof(ObtenerMedico), new { id = creado.Id },
            ApiResponse<MedicoResponseDto>.Ok(creado, "Médico registrado correctamente."));
    }

    /// <summary>Actualiza un médico (incluye activarlo o desactivarlo).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] MedicoRequestDto dto) =>
        Ok(ApiResponse<MedicoResponseDto>.Ok(await _medicos.ActualizarAsync(id, dto), "Médico actualizado correctamente."));

    /// <summary>Elimina un médico que no tenga citas registradas.</summary>
    /// <response code="204">Médico eliminado.</response>
    /// <response code="409">El médico tiene citas registradas.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _medicos.EliminarAsync(id);
        return NoContent();
    }

    /// <summary>Lista las citas de un médico, opcionalmente de un día (ruta jerárquica).</summary>
    /// <param name="id">Id del médico.</param>
    /// <param name="fecha">Día a consultar (yyyy-MM-dd), opcional.</param>
    [HttpGet("{id:int}/citas")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CitaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarCitas(int id, [FromQuery] DateOnly? fecha)
    {
        var datos = await _citas.ListarAsync(new CitaFiltroDto { MedicoId = id, Fecha = fecha });
        return Ok(ApiResponse<IReadOnlyList<CitaResponseDto>>.Ok(datos, $"El médico tiene {datos.Count} cita(s)."));
    }

    /// <summary>Horarios libres de un médico en un día (bloques de 30 minutos).</summary>
    /// <param name="id">Id del médico.</param>
    /// <param name="fecha">Día a consultar (yyyy-MM-dd).</param>
    [HttpGet("{id:int}/disponibilidad")]
    [ProducesResponseType(typeof(ApiResponse<DisponibilidadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Disponibilidad(int id, [FromQuery, Required(ErrorMessage = "El parámetro 'fecha' es obligatorio (yyyy-MM-dd).")] DateOnly? fecha)
    {
        var datos = await _medicos.ObtenerDisponibilidadAsync(id, fecha!.Value);
        return Ok(ApiResponse<DisponibilidadDto>.Ok(datos, $"{datos.HorariosDisponibles.Count} horario(s) disponible(s)."));
    }
}
