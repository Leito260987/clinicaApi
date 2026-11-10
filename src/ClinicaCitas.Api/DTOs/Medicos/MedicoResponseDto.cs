namespace ClinicaCitas.Api.DTOs.Medicos;

/// <summary>Médico tal como lo devuelve la API.</summary>
public class MedicoResponseDto
{
    public int Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string NombreCompleto { get; init; } = string.Empty;
    public string Dni { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Cmp { get; init; } = string.Empty;
    public string Especialidad { get; init; } = string.Empty;
    public bool Activo { get; init; }
    public DateTime FechaRegistro { get; init; }
    public DateTime? FechaActualizacion { get; init; }
}
