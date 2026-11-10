using System.ComponentModel.DataAnnotations;

namespace ClinicaCitas.Api.Validation;

/// <summary>Valida que la fecha de nacimiento sea pasada y razonable (máximo 120 años).</summary>
[AttributeUsage(AttributeTargets.Property)]
public class FechaNacimientoValidaAttribute : ValidationAttribute
{
    public FechaNacimientoValidaAttribute()
        : base("La fecha de nacimiento debe ser anterior a hoy y no mayor a 120 años.") { }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;
        return value is DateTime fecha && fecha.Date < DateTime.Today && fecha.Date > DateTime.Today.AddYears(-120)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage, new[] { validationContext.MemberName! });
    }
}
