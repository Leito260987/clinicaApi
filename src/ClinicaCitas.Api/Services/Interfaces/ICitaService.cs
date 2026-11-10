using ClinicaCitas.Api.DTOs.Citas;

namespace ClinicaCitas.Api.Services.Interfaces;

/// <summary>Lógica de negocio de citas médicas.</summary>
public interface ICitaService
{
    Task<IReadOnlyList<CitaResponseDto>> ListarAsync(CitaFiltroDto filtro);
    Task<CitaResponseDto> ObtenerAsync(int id);
    Task<CitaResponseDto> CrearAsync(CitaCrearDto dto);
    Task<CitaResponseDto> ReprogramarAsync(int id, CitaActualizarDto dto);
    Task<CitaResponseDto> CancelarAsync(int id, CitaCancelarDto dto);
    Task<CitaResponseDto> MarcarAtendidaAsync(int id);
    Task EliminarAsync(int id);
}
