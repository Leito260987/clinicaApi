using ClinicaCitas.Api.DTOs.Medicos;
using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.Services.Interfaces;

/// <summary>Lógica de negocio de médicos.</summary>
public interface IMedicoService
{
    Task<IReadOnlyList<MedicoResponseDto>> ListarAsync(Especialidad? especialidad);
    Task<MedicoResponseDto> ObtenerAsync(int id);
    Task<MedicoResponseDto> CrearAsync(MedicoRequestDto dto);
    Task<MedicoResponseDto> ActualizarAsync(int id, MedicoRequestDto dto);
    Task EliminarAsync(int id);
    Task<DisponibilidadDto> ObtenerDisponibilidadAsync(int id, DateOnly fecha);
}
