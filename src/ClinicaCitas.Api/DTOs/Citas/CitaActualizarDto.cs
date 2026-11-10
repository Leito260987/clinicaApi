using System.ComponentModel.DataAnnotations;
using ClinicaCitas.Api.Validation;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>Datos para reprogramar una cita (médico, fecha y motivo).</summary>
public class CitaActualizarDto
{
    /// <example>1</example>
    [Required(ErrorMessage = "El id del médico es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El id del médico debe ser mayor que 0.")]
    public int? MedicoId { get; init; }

    /// <example>2026-11-16T11:30:00</example>
    [Required(ErrorMessage = "La fecha y hora de la cita es obligatoria.")]
    [FechaFutura]
    public DateTime? FechaHora { get; init; }

    /// <example>Control de presión arterial (reprogramada)</example>
    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El motivo debe tener entre 5 y 200 caracteres.")]
    public string Motivo { get; init; } = string.Empty;
}
