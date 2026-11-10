using System.ComponentModel.DataAnnotations;
using ClinicaCitas.Api.Validation;

namespace ClinicaCitas.Api.DTOs.Pacientes;

/// <summary>Datos para registrar o actualizar un paciente.</summary>
public class PacienteRequestDto
{
    /// <example>María Elena</example>
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 60 caracteres.")]
    public string Nombres { get; init; } = string.Empty;

    /// <example>Quispe Torres</example>
    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 60 caracteres.")]
    public string Apellidos { get; init; } = string.Empty;

    /// <example>45879632</example>
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    public string Dni { get; init; } = string.Empty;

    /// <example>maria.quispe@correo.com</example>
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(100, ErrorMessage = "El correo no debe superar los 100 caracteres.")]
    public string Email { get; init; } = string.Empty;

    /// <example>987654321</example>
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe ser un celular de 9 dígitos que empiece en 9.")]
    public string Telefono { get; init; } = string.Empty;

    /// <example>1990-05-14</example>
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [FechaNacimientoValida]
    public DateTime? FechaNacimiento { get; init; }
}
