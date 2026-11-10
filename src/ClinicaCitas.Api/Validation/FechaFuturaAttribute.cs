using System.ComponentModel.DataAnnotations;

namespace ClinicaCitas.Api.Validation;

/// <summary>Valida que la fecha y hora sea posterior al momento actual.</summary>
[AttributeUsage(AttributeTargets.Property)]
public class FechaFuturaAttribute : ValidationAttribute
{
    public FechaFuturaAttribute() : base("La fecha y hora de la cita debe ser futura.") { }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success; // [Required] se encarga del valor vacío
        return value is DateTime fecha && fecha > DateTime.Now
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage, new[] { validationContext.MemberName! });
    }
}
