using ClinicaCitas.Api.Models;

namespace ClinicaCitas.Api.Repositories.Interfaces;

/// <summary>
/// Contrato de acceso a datos. Hoy lo implementa un repositorio en memoria;
/// mañana podría implementarlo Entity Framework sin cambiar los servicios.
/// </summary>
public interface IRepository<T> where T : EntidadBase
{
    Task<IReadOnlyList<T>> ObtenerTodosAsync();
    Task<T?> ObtenerPorIdAsync(int id);
    Task<IReadOnlyList<T>> BuscarAsync(Func<T, bool> filtro);
    Task<bool> ExisteAsync(Func<T, bool> filtro);
    Task<T> AgregarAsync(T entidad);
    Task ActualizarAsync(T entidad);
    Task<bool> EliminarAsync(int id);
}
