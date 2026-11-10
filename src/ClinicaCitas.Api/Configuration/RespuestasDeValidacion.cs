using ClinicaCitas.Api.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaCitas.Api.Configuration;

/// <summary>
/// Reemplaza la respuesta 400 por defecto de ASP.NET Core para que los errores
/// de validación ([Required], [StringLength], etc.) usen el mismo formato que el resto de errores.
/// </summary>
public static class RespuestasDeValidacion
{
    public static IActionResult Crear(ActionContext contexto)
    {
        var errores = contexto.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                e => NombreCampo(e.Key),
                e => e.Value!.Errors.Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage)
                    ? "El valor enviado no tiene el formato esperado."
                    : TraducirMensaje(x.ErrorMessage)).ToArray());

        // Si un campo concreto falló, el aviso genérico sobre el cuerpo sobra
        if (errores.Count > 1) errores.Remove("cuerpo");

        var http = contexto.HttpContext;
        return new BadRequestObjectResult(new ErrorResponse
        {
            Estado = StatusCodes.Status400BadRequest,
            Error = "Datos de entrada inválidos",
            Mensaje = "Revise los campos indicados en 'errores'.",
            Errores = errores,
            Ruta = $"{http.Request.Method} {http.Request.Path}",
            TraceId = http.TraceIdentifier
        });
    }

    // "$.fechaHora" o "dto.FechaHora" -> "fechaHora"
    private static string NombreCampo(string clave)
    {
        var limpio = clave.Replace("$.", string.Empty);
        if (limpio.StartsWith("dto.", StringComparison.OrdinalIgnoreCase)) limpio = limpio[4..];
        if (string.IsNullOrEmpty(limpio) || limpio == "$" || limpio.Equals("dto", StringComparison.OrdinalIgnoreCase)) return "cuerpo";
        return char.ToLowerInvariant(limpio[0]) + limpio[1..];
    }

    // Mensajes del deserializador JSON, que vienen en inglés
    private static string TraducirMensaje(string mensaje) =>
        mensaje.StartsWith("The JSON value could not be converted", StringComparison.Ordinal) ||
        mensaje.StartsWith("The input was not valid", StringComparison.Ordinal) ||
        mensaje.Contains("is an invalid", StringComparison.Ordinal) ||
        mensaje.Contains("no es una fecha válida", StringComparison.Ordinal)
            ? "El valor enviado no tiene el formato o tipo esperado."
            : mensaje.Contains("field is required", StringComparison.Ordinal)
                ? "El cuerpo de la solicitud falta o no es un JSON válido."
                : mensaje;
}
