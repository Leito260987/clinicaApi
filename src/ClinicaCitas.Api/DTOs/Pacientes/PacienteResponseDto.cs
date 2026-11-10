namespace ClinicaCitas.Api.DTOs.Pacientes;

/// <summary>Paciente tal como lo devuelve la API.</summary>
public class PacienteResponseDto
{
    public int Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string NombreCompleto { get; init; } = string.Empty;
    public string Dni { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public DateTime FechaNacimiento { get; init; }
    public int Edad { get; init; }
    public DateTime FechaRegistro { get; init; }
    public DateTime? FechaActualizacion { get; init; }
}
