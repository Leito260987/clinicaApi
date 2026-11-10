using ClinicaCitas.Api.Models;
using ClinicaCitas.Api.Repositories.Interfaces;

namespace ClinicaCitas.Api.Repositories;

/// <summary>
/// Simula una tabla de base de datos con una <see cref="List{T}"/> en memoria:
/// genera identificadores autoincrementales, protege el acceso concurrente con un
/// candado (lock) y devuelve listas de solo lectura. Los datos se pierden al detener la API.
/// </summary>
public class InMemoryRepository<T> : IRepository<T> where T : EntidadBase
{
    private readonly List<T> _datos = new();
    private readonly object _candado = new();
    private int _ultimoId;

    public Task<IReadOnlyList<T>> ObtenerTodosAsync()
    {
        lock (_candado)
            return Task.FromResult<IReadOnlyList<T>>(_datos.OrderBy(e => e.Id).ToList());
    }

    public Task<T?> ObtenerPorIdAsync(int id)
    {
        lock (_candado)
            return Task.FromResult(_datos.FirstOrDefault(e => e.Id == id));
    }

    public Task<IReadOnlyList<T>> BuscarAsync(Func<T, bool> filtro)
    {
        lock (_candado)
            return Task.FromResult<IReadOnlyList<T>>(_datos.Where(filtro).OrderBy(e => e.Id).ToList());
    }

    public Task<bool> ExisteAsync(Func<T, bool> filtro)
    {
        lock (_candado)
            return Task.FromResult(_datos.Any(filtro));
    }

    public Task<T> AgregarAsync(T entidad)
    {
        lock (_candado)
        {
            _ultimoId++;                                   // equivalente a IDENTITY(1,1)
            entidad.AsignarIdentidad(_ultimoId, DateTime.Now);
            _datos.Add(entidad);
            return Task.FromResult(entidad);
        }
    }

    public Task ActualizarAsync(T entidad)
    {
        lock (_candado)
        {
            var indice = _datos.FindIndex(e => e.Id == entidad.Id);
            if (indice < 0)
                throw new InvalidOperationException($"No se puede actualizar: la entidad {entidad.Id} no está registrada.");
            _datos[indice] = entidad;
            return Task.CompletedTask;
        }
    }

    public Task<bool> EliminarAsync(int id)
    {
        lock (_candado)
            return Task.FromResult(_datos.RemoveAll(e => e.Id == id) > 0);
    }
}
