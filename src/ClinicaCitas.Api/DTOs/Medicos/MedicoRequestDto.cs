using System.ComponentModel.DataAnnotations;
using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.DTOs.Medicos;

/// <summary>Datos para registrar o actualizar un médico.</summary>
public class MedicoRequestDto
{
    /// <example>Jorge Luis</example>
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 60 caracteres.")]
    public string Nombres { get; init; } = string.Empty;

    /// <example>Ramírez Soto</example>
    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 60 caracteres.")]
    public string Apellidos { get; init; } = string.Empty;

    /// <example>40125896</example>
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    public string Dni { get; init; } = string.Empty;

    /// <example>jramirez@clinica.pe</example>
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(100, ErrorMessage = "El correo no debe superar los 100 caracteres.")]
    public string Email { get; init; } = string.Empty;

    /// <example>912345678</example>
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe ser un celular de 9 dígitos que empiece en 9.")]
    public string Telefono { get; init; } = string.Empty;

    /// <summary>Número de colegiatura (CMP) de 5 o 6 dígitos.</summary>
    /// <example>045678</example>
    [Required(ErrorMessage = "El CMP es obligatorio.")]
    [RegularExpression(@"^\d{5,6}$", ErrorMessage = "El CMP debe tener 5 o 6 dígitos.")]
    public string Cmp { get; init; } = string.Empty;

    /// <example>Cardiologia</example>
    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [EnumDataType(typeof(Especialidad), ErrorMessage = "La especialidad no es válida.")]
    public Especialidad? Especialidad { get; init; }

    /// <summary>Solo se usa al actualizar; por defecto el médico queda activo.</summary>
    /// <example>true</example>
    public bool Activo { get; init; } = true;
}
