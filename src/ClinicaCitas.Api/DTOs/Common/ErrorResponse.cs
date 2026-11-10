namespace ClinicaCitas.Api.DTOs.Common;

/// <summary>Estructura única para todos los errores que devuelve la API.</summary>
public class ErrorResponse
{
    public bool Exito { get; init; } = false;
    /// <summary>Código de estado HTTP (400, 404, 409, 422, 500).</summary>
    public int Estado { get; init; }
    /// <summary>Tipo de error en pocas palabras.</summary>
    public string Error { get; init; } = string.Empty;
    /// <summary>Explicación para quien consume la API.</summary>
    public string Mensaje { get; init; } = string.Empty;
    /// <summary>Errores de validación por campo (solo en respuestas 400).</summary>
    public IDictionary<string, string[]>? Errores { get; init; }
    public string Ruta { get; init; } = string.Empty;
    public DateTime Fecha { get; init; } = DateTime.Now;
    public string? TraceId { get; init; }
}
