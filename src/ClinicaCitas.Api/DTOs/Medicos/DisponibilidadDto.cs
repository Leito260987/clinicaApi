namespace ClinicaCitas.Api.DTOs.Medicos;

/// <summary>Horarios libres de un médico en un día.</summary>
public class DisponibilidadDto
{
    public int MedicoId { get; init; }
    public string Medico { get; init; } = string.Empty;
    public DateOnly Fecha { get; init; }
    public int DuracionCitaMinutos { get; init; }
    public IReadOnlyList<string> HorariosDisponibles { get; init; } = Array.Empty<string>();
}
