namespace ClinicaCitas.Api.DTOs.Common;

/// <summary>Envoltorio uniforme para todas las respuestas exitosas.</summary>
public class ApiResponse<T>
{
    public bool Exito { get; init; } = true;
    public string Mensaje { get; init; } = string.Empty;
    public T? Datos { get; init; }

    public static ApiResponse<T> Ok(T datos, string mensaje) => new() { Datos = datos, Mensaje = mensaje };
}
