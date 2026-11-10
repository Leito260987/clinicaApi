namespace ClinicaCitas.Api.Models;

/// <summary>
/// Clase base de todas las entidades persistidas en memoria.
/// Centraliza el identificador y las fechas de auditoría.
/// </summary>
public abstract class EntidadBase
{
    /// <summary>Identificador único asignado por el repositorio (simula una columna IDENTITY).</summary>
    public int Id { get; private set; }

    /// <summary>Fecha y hora en que se registró la entidad.</summary>
    public DateTime FechaRegistro { get; private set; }

    /// <summary>Fecha y hora de la última modificación (null si nunca se modificó).</summary>
    public DateTime? FechaActualizacion { get; private set; }

    /// <summary>Lo usa únicamente el repositorio al insertar la entidad.</summary>
    internal void AsignarIdentidad(int id, DateTime fechaRegistro)
    {
        Id = id;
        FechaRegistro = fechaRegistro;
    }

    /// <summary>Marca la entidad como modificada.</summary>
    protected void MarcarActualizada() => FechaActualizacion = DateTime.Now;
}
