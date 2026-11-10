using System.Text.Json;
using ClinicaCitas.Api.DTOs.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace ClinicaCitas.Api.Configuration;

/// <summary>Da formato de error uniforme a respuestas sin cuerpo (ruta inexistente, método no permitido, etc.).</summary>
public static class RespuestasDeEstado
{
    public static async Task EscribirAsync(StatusCodeContext contexto)
    {
        var http = contexto.HttpContext;
        var estado = http.Response.StatusCode;
        var (titulo, mensaje) = estado switch
        {
            404 => ("Ruta no encontrada", "La ruta solicitada no existe. Revise la documentación en /swagger."),
            405 => ("Método no permitido", $"El método {http.Request.Method} no está permitido en esta ruta."),
            415 => ("Tipo de contenido no soportado", "Envíe el cuerpo en formato JSON (Content-Type: application/json)."),
            _ => ("Error", "La solicitud no pudo completarse.")
        };
        var json = http.RequestServices.GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>().Value.JsonSerializerOptions;
        http.Response.ContentType = "application/json; charset=utf-8";
        await http.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse
        {
            Estado = estado,
            Error = titulo,
            Mensaje = mensaje,
            Ruta = $"{http.Request.Method} {http.Request.Path}",
            TraceId = http.TraceIdentifier
        }, json));
    }
}
