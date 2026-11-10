namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>Cita tal como la devuelve la API, con datos resumidos de paciente y médico.</summary>
public class CitaResponseDto
{
    public int Id { get; init; }
    public DateTime FechaHora { get; init; }
    public DateTime FechaHoraFin { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string? MotivoCancelacion { get; init; }
    public PersonaResumenDto Paciente { get; init; } = new();
    public MedicoResumenDto Medico { get; init; } = new();
    public DateTime FechaRegistro { get; init; }
    public DateTime? FechaActualizacion { get; init; }
}

public class PersonaResumenDto
{
    public int Id { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Dni { get; init; } = string.Empty;
}

public class MedicoResumenDto
{
    public int Id { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Especialidad { get; init; } = string.Empty;
}
