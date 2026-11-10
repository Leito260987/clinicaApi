using System.ComponentModel.DataAnnotations;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>Motivo por el que se cancela una cita.</summary>
public class CitaCancelarDto
{
    /// <example>El paciente no podrá asistir por viaje</example>
    [Required(ErrorMessage = "El motivo de cancelación es obligatorio.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El motivo de cancelación debe tener entre 5 y 200 caracteres.")]
    public string MotivoCancelacion { get; init; } = string.Empty;
}
