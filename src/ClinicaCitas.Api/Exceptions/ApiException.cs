namespace ClinicaCitas.Api.Exceptions;

/// <summary>
/// Excepción base de la API. Cada excepción derivada define su código HTTP,
/// y el middleware la convierte en una respuesta de error con formato uniforme.
/// </summary>
public abstract class ApiException : Exception
{
    public int CodigoEstado { get; }
    public string Titulo { get; }

    protected ApiException(int codigoEstado, string titulo, string mensaje) : base(mensaje)
    {
        CodigoEstado = codigoEstado;
        Titulo = titulo;
    }
}

/// <summary>404: el recurso solicitado no existe.</summary>
public class RecursoNoEncontradoException : ApiException
{
    public RecursoNoEncontradoException(string recurso, int id)
        : base(StatusCodes.Status404NotFound, "Recurso no encontrado", $"No existe {recurso} con id {id}.") { }
}

/// <summary>409: la operación choca con datos existentes (duplicados u horarios ocupados).</summary>
public class ConflictoException : ApiException
{
    public ConflictoException(string mensaje)
        : base(StatusCodes.Status409Conflict, "Conflicto con datos existentes", mensaje) { }
}

/// <summary>422: los datos son válidos pero la operación incumple una regla del negocio.</summary>
public class ReglaNegocioException : ApiException
{
    public ReglaNegocioException(string mensaje)
        : base(StatusCodes.Status422UnprocessableEntity, "Regla de negocio incumplida", mensaje) { }
}
