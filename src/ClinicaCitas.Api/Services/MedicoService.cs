using ClinicaCitas.Api.DTOs.Medicos;
using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Mappers;
using ClinicaCitas.Api.Models;
using ClinicaCitas.Api.Models.Enums;
using ClinicaCitas.Api.Repositories.Interfaces;
using ClinicaCitas.Api.Services.Interfaces;

namespace ClinicaCitas.Api.Services;

public class MedicoService : IMedicoService
{
    private readonly IRepository<Medico> _medicos;
    private readonly IRepository<Cita> _citas;

    public MedicoService(IRepository<Medico> medicos, IRepository<Cita> citas)
    {
        _medicos = medicos;
        _citas = citas;
    }

    public async Task<IReadOnlyList<MedicoResponseDto>> ListarAsync(Especialidad? especialidad)
    {
        var lista = especialidad is null
            ? await _medicos.ObtenerTodosAsync()
            : await _medicos.BuscarAsync(m => m.Especialidad == especialidad);
        return lista.Select(m => m.ADto()).ToList();
    }

    public async Task<MedicoResponseDto> ObtenerAsync(int id) => (await BuscarOFallarAsync(id)).ADto();

    public async Task<MedicoResponseDto> CrearAsync(MedicoRequestDto dto)
    {
        await ValidarUnicosAsync(dto, idActual: null);
        var medico = new Medico(dto.Nombres, dto.Apellidos, dto.Dni, dto.Email, dto.Telefono, dto.Cmp, dto.Especialidad!.Value);
        return (await _medicos.AgregarAsync(medico)).ADto();
    }

    public async Task<MedicoResponseDto> ActualizarAsync(int id, MedicoRequestDto dto)
    {
        var medico = await BuscarOFallarAsync(id);
        await ValidarUnicosAsync(dto, idActual: id);
        medico.Actualizar(dto.Nombres, dto.Apellidos, dto.Dni, dto.Email, dto.Telefono, dto.Cmp, dto.Especialidad!.Value, dto.Activo);
        await _medicos.ActualizarAsync(medico);
        return medico.ADto();
    }

    public async Task EliminarAsync(int id)
    {
        await BuscarOFallarAsync(id);
        var citas = await _citas.BuscarAsync(c => c.MedicoId == id);
        if (citas.Count > 0)
            throw new ConflictoException(
                $"No se puede eliminar el médico {id} porque tiene {citas.Count} cita(s) registrada(s). Puede desactivarlo con PUT (activo = false).");
        await _medicos.EliminarAsync(id);
    }

    public async Task<DisponibilidadDto> ObtenerDisponibilidadAsync(int id, DateOnly fecha)
    {
        var medico = await BuscarOFallarAsync(id);
        if (fecha < DateOnly.FromDateTime(DateTime.Today))
            throw new ReglaNegocioException("No se puede consultar disponibilidad de una fecha pasada.");

        var ocupadas = await _citas.BuscarAsync(c =>
            c.MedicoId == id && c.OcupaHorario && DateOnly.FromDateTime(c.FechaHora) == fecha);

        var libres = HorarioAtencion.BloquesDelDia(fecha)
            .Where(inicio => inicio > DateTime.Now)
            .Where(inicio => !ocupadas.Any(c => c.SeCruzaCon(inicio, inicio.AddMinutes(Cita.DuracionMinutos))))
            .Select(inicio => inicio.ToString("HH:mm"))
            .ToList();

        return new DisponibilidadDto
        {
            MedicoId = medico.Id,
            Medico = medico.NombreCompleto,
            Fecha = fecha,
            DuracionCitaMinutos = Cita.DuracionMinutos,
            HorariosDisponibles = medico.Activo ? libres : Array.Empty<string>()
        };
    }

    private async Task<Medico> BuscarOFallarAsync(int id) =>
        await _medicos.ObtenerPorIdAsync(id) ?? throw new RecursoNoEncontradoException("un médico", id);

    private async Task ValidarUnicosAsync(MedicoRequestDto dto, int? idActual)
    {
        if (await _medicos.ExisteAsync(m => m.Cmp == dto.Cmp.Trim() && m.Id != idActual))
            throw new ConflictoException($"Ya existe un médico registrado con el CMP {dto.Cmp}.");
        if (await _medicos.ExisteAsync(m => m.Dni == dto.Dni.Trim() && m.Id != idActual))
            throw new ConflictoException($"Ya existe un médico registrado con el DNI {dto.Dni}.");
    }
}
