using ClinicaCitas.Api.DTOs.Pacientes;
using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Mappers;
using ClinicaCitas.Api.Models;
using ClinicaCitas.Api.Repositories.Interfaces;
using ClinicaCitas.Api.Services.Interfaces;

namespace ClinicaCitas.Api.Services;

public class PacienteService : IPacienteService
{
    private readonly IRepository<Paciente> _pacientes;
    private readonly IRepository<Cita> _citas;

    public PacienteService(IRepository<Paciente> pacientes, IRepository<Cita> citas)
    {
        _pacientes = pacientes;
        _citas = citas;
    }

    public async Task<IReadOnlyList<PacienteResponseDto>> ListarAsync(string? busqueda)
    {
        var lista = string.IsNullOrWhiteSpace(busqueda)
            ? await _pacientes.ObtenerTodosAsync()
            : await _pacientes.BuscarAsync(p =>
                p.NombreCompleto.Contains(busqueda.Trim(), StringComparison.OrdinalIgnoreCase) ||
                p.Dni.Contains(busqueda.Trim()));
        return lista.Select(p => p.ADto()).ToList();
    }

    public async Task<PacienteResponseDto> ObtenerAsync(int id) => (await BuscarOFallarAsync(id)).ADto();

    public async Task<PacienteResponseDto> CrearAsync(PacienteRequestDto dto)
    {
        await ValidarDniUnicoAsync(dto.Dni, idActual: null);
        var paciente = new Paciente(dto.Nombres, dto.Apellidos, dto.Dni, dto.Email, dto.Telefono, dto.FechaNacimiento!.Value);
        return (await _pacientes.AgregarAsync(paciente)).ADto();
    }

    public async Task<PacienteResponseDto> ActualizarAsync(int id, PacienteRequestDto dto)
    {
        var paciente = await BuscarOFallarAsync(id);
        await ValidarDniUnicoAsync(dto.Dni, idActual: id);
        paciente.Actualizar(dto.Nombres, dto.Apellidos, dto.Dni, dto.Email, dto.Telefono, dto.FechaNacimiento!.Value);
        await _pacientes.ActualizarAsync(paciente);
        return paciente.ADto();
    }

    public async Task EliminarAsync(int id)
    {
        await BuscarOFallarAsync(id);
        var citas = await _citas.BuscarAsync(c => c.PacienteId == id);
        if (citas.Count > 0)
            throw new ConflictoException(
                $"No se puede eliminar el paciente {id} porque tiene {citas.Count} cita(s) registrada(s). Elimine primero sus citas.");
        await _pacientes.EliminarAsync(id);
    }

    private async Task<Paciente> BuscarOFallarAsync(int id) =>
        await _pacientes.ObtenerPorIdAsync(id) ?? throw new RecursoNoEncontradoException("un paciente", id);

    private async Task ValidarDniUnicoAsync(string dni, int? idActual)
    {
        if (await _pacientes.ExisteAsync(p => p.Dni == dni.Trim() && p.Id != idActual))
            throw new ConflictoException($"Ya existe un paciente registrado con el DNI {dni}.");
    }
}
