using ClinicaCitas.Api.DTOs.Pacientes;

namespace ClinicaCitas.Api.Services.Interfaces;

/// <summary>Lógica de negocio de pacientes.</summary>
public interface IPacienteService
{
    Task<IReadOnlyList<PacienteResponseDto>> ListarAsync(string? busqueda);
    Task<PacienteResponseDto> ObtenerAsync(int id);
    Task<PacienteResponseDto> CrearAsync(PacienteRequestDto dto);
    Task<PacienteResponseDto> ActualizarAsync(int id, PacienteRequestDto dto);
    Task EliminarAsync(int id);
}
