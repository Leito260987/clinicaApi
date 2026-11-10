using System.Text.Json;
using ClinicaCitas.Api.DTOs.Common;
using ClinicaCitas.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace ClinicaCitas.Api.Middleware;

/// <summary>
/// Manejo centralizado de excepciones: ningún controlador usa try/catch.
/// Las excepciones de la API se traducen a su código HTTP y cualquier otra
/// a un 500 genérico (sin exponer detalles internos al cliente).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly JsonSerializerOptions _json;

    public ExceptionHandlingMiddleware(RequestDelegate siguiente, ILogger<ExceptionHandlingMiddleware> logger,
        IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> opcionesJson)
    {
        _siguiente = siguiente;
        _logger = logger;
        _json = opcionesJson.Value.JsonSerializerOptions;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("{Titulo}: {Mensaje}", ex.Titulo, ex.Message);
            await EscribirErrorAsync(contexto, ex.CodigoEstado, ex.Titulo, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en {Ruta}", contexto.Request.Path);
            await EscribirErrorAsync(contexto, StatusCodes.Status500InternalServerError, "Error interno del servidor",
                "Ocurrió un error inesperado. Intente nuevamente; si persiste, contacte al administrador.");
        }
    }

    private async Task EscribirErrorAsync(HttpContext contexto, int estado, string titulo, string mensaje)
    {
        if (contexto.Response.HasStarted) return;
        contexto.Response.Clear();
        contexto.Response.StatusCode = estado;
        contexto.Response.ContentType = "application/json; charset=utf-8";
        var error = new ErrorResponse
        {
            Estado = estado,
            Error = titulo,
            Mensaje = mensaje,
            Ruta = $"{contexto.Request.Method} {contexto.Request.Path}",
            TraceId = contexto.TraceIdentifier
        };
        await contexto.Response.WriteAsync(JsonSerializer.Serialize(error, _json));
    }
}
