using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>Filtros opcionales para listar citas (se envían por query string).</summary>
public class CitaFiltroDto
{
    /// <summary>Solo citas de este paciente.</summary>
    public int? PacienteId { get; init; }
    /// <summary>Solo citas de este médico.</summary>
    public int? MedicoId { get; init; }
    /// <summary>Programada, Cancelada o Atendida.</summary>
    public EstadoCita? Estado { get; init; }
    /// <summary>Solo citas de este día (formato yyyy-MM-dd).</summary>
    public DateOnly? Fecha { get; init; }
}
