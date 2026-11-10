namespace ClinicaCitas.Api.Models;

/// <summary>
/// Datos comunes a pacientes y médicos. Se reutilizan mediante herencia.
/// </summary>
public abstract class Persona : EntidadBase
{
    public string Nombres { get; private set; }
    public string Apellidos { get; private set; }
    public string Dni { get; private set; }
    public string Email { get; private set; }
    public string Telefono { get; private set; }

    /// <summary>Nombre para mostrar en respuestas y reportes.</summary>
    public string NombreCompleto => $"{Nombres} {Apellidos}";

    protected Persona(string nombres, string apellidos, string dni, string email, string telefono)
    {
        Nombres = Normalizar(nombres);
        Apellidos = Normalizar(apellidos);
        Dni = dni.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefono = telefono.Trim();
    }

    /// <summary>Actualiza los datos personales y registra la fecha de modificación.</summary>
    protected void ActualizarDatosPersonales(string nombres, string apellidos, string dni, string email, string telefono)
    {
        Nombres = Normalizar(nombres);
        Apellidos = Normalizar(apellidos);
        Dni = dni.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefono = telefono.Trim();
        MarcarActualizada();
    }

    private static string Normalizar(string texto) =>
        string.Join(' ', texto.Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
