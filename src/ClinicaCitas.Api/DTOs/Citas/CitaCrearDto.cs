using System.ComponentModel.DataAnnotations;
using ClinicaCitas.Api.Validation;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>Datos para registrar una nueva cita.</summary>
public class CitaCrearDto
{
    /// <example>1</example>
    [Required(ErrorMessage = "El id del paciente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El id del paciente debe ser mayor que 0.")]
    public int? PacienteId { get; init; }

    /// <example>1</example>
    [Required(ErrorMessage = "El id del médico es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El id del médico debe ser mayor que 0.")]
    public int? MedicoId { get; init; }

    /// <summary>Fecha y hora de inicio (hora local). Lunes a sábado, de 08:00 a 19:30, en bloques de 30 minutos.</summary>
    /// <example>2026-11-16T10:00:00</example>
    [Required(ErrorMessage = "La fecha y hora de la cita es obligatoria.")]
    [FechaFutura]
    public DateTime? FechaHora { get; init; }

    /// <example>Control de presión arterial</example>
    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "El motivo debe tener entre 5 y 200 caracteres.")]
    public string Motivo { get; init; } = string.Empty;
}
