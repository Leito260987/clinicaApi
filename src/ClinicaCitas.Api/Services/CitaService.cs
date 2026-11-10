using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Mappers;
using ClinicaCitas.Api.Models;
using ClinicaCitas.Api.Models.Enums;
using ClinicaCitas.Api.Repositories.Interfaces;
using ClinicaCitas.Api.Services.Interfaces;

namespace ClinicaCitas.Api.Services;

/// <summary>
/// Reglas de negocio de las citas: horario de atención, médico activo,
/// sin cruces de horario para el médico ni para el paciente, y estados válidos.
/// </summary>
public class CitaService : ICitaService
{
    private readonly IRepository<Cita> _citas;
    private readonly IRepository<Paciente> _pacientes;
    private readonly IRepository<Medico> _medicos;

    public CitaService(IRepository<Cita> citas, IRepository<Paciente> pacientes, IRepository<Medico> medicos)
    {
        _citas = citas;
        _pacientes = pacientes;
        _medicos = medicos;
    }

    public async Task<IReadOnlyList<CitaResponseDto>> ListarAsync(CitaFiltroDto filtro)
    {
        if (filtro.PacienteId is int pid && await _pacientes.ObtenerPorIdAsync(pid) is null)
            throw new RecursoNoEncontradoException("un paciente", pid);
        if (filtro.MedicoId is int mid && await _medicos.ObtenerPorIdAsync(mid) is null)
            throw new RecursoNoEncontradoException("un médico", mid);

        var citas = await _citas.BuscarAsync(c =>
            (filtro.PacienteId is null || c.PacienteId == filtro.PacienteId) &&
            (filtro.MedicoId is null || c.MedicoId == filtro.MedicoId) &&
            (filtro.Estado is null || c.Estado == filtro.Estado) &&
            (filtro.Fecha is null || DateOnly.FromDateTime(c.FechaHora) == filtro.Fecha));

        return await ADtosAsync(citas.OrderBy(c => c.FechaHora));
    }

    public async Task<CitaResponseDto> ObtenerAsync(int id) => await ADtoAsync(await BuscarOFallarAsync(id));

    public async Task<CitaResponseDto> CrearAsync(CitaCrearDto dto)
    {
        var paciente = await _pacientes.ObtenerPorIdAsync(dto.PacienteId!.Value)
                       ?? throw new RecursoNoEncontradoException("un paciente", dto.PacienteId.Value);
        var medico = await ObtenerMedicoActivoAsync(dto.MedicoId!.Value);
        var fechaHora = dto.FechaHora!.Value;

        HorarioAtencion.Validar(fechaHora);
        await ValidarDisponibilidadAsync(medico.Id, paciente.Id, fechaHora, citaIgnorada: null);

        var cita = await _citas.AgregarAsync(new Cita(paciente.Id, medico.Id, fechaHora, dto.Motivo));
        return cita.ADto(paciente, medico);
    }

    public async Task<CitaResponseDto> ReprogramarAsync(int id, CitaActualizarDto dto)
    {
        var cita = await BuscarOFallarAsync(id);
        cita.AsegurarQueEsteProgramada("reprogramar");        // primero el estado, luego horario y cruces
        var medico = await ObtenerMedicoActivoAsync(dto.MedicoId!.Value);
        var fechaHora = dto.FechaHora!.Value;

        HorarioAtencion.Validar(fechaHora);
        await ValidarDisponibilidadAsync(medico.Id, cita.PacienteId, fechaHora, citaIgnorada: cita.Id);

        cita.Reprogramar(medico.Id, fechaHora, dto.Motivo);
        await _citas.ActualizarAsync(cita);
        return await ADtoAsync(cita);
    }

    public async Task<CitaResponseDto> CancelarAsync(int id, CitaCancelarDto dto)
    {
        var cita = await BuscarOFallarAsync(id);
        cita.Cancelar(dto.MotivoCancelacion);
        await _citas.ActualizarAsync(cita);
        return await ADtoAsync(cita);
    }

    public async Task<CitaResponseDto> MarcarAtendidaAsync(int id)
    {
        var cita = await BuscarOFallarAsync(id);
        cita.MarcarAtendida();
        await _citas.ActualizarAsync(cita);
        return await ADtoAsync(cita);
    }

    public async Task EliminarAsync(int id)
    {
        var cita = await BuscarOFallarAsync(id);
        if (cita.Estado == EstadoCita.Atendida)
            throw new ReglaNegocioException(
                $"La cita {id} ya fue atendida y forma parte del historial del paciente; no se puede eliminar.");
        await _citas.EliminarAsync(id);
    }

    // ---------------------------------------------------------------- auxiliares

    private async Task<Cita> BuscarOFallarAsync(int id) =>
        await _citas.ObtenerPorIdAsync(id) ?? throw new RecursoNoEncontradoException("una cita", id);

    private async Task<Medico> ObtenerMedicoActivoAsync(int medicoId)
    {
        var medico = await _medicos.ObtenerPorIdAsync(medicoId)
                     ?? throw new RecursoNoEncontradoException("un médico", medicoId);
        if (!medico.Activo)
            throw new ReglaNegocioException($"El médico {medico.NombreCompleto} no está activo y no puede recibir citas.");
        return medico;
    }

    /// <summary>Un médico y un paciente no pueden tener dos citas que se crucen.</summary>
    private async Task ValidarDisponibilidadAsync(int medicoId, int pacienteId, DateTime inicio, int? citaIgnorada)
    {
        var fin = inicio.AddMinutes(Cita.DuracionMinutos);

        if (await _citas.ExisteAsync(c => c.Id != citaIgnorada && c.MedicoId == medicoId && c.OcupaHorario && c.SeCruzaCon(inicio, fin)))
            throw new ConflictoException($"El médico ya tiene una cita el {inicio:dd/MM/yyyy} a las {inicio:HH:mm}. " +
                                         $"Consulte GET /api/medicos/{medicoId}/disponibilidad?fecha={inicio:yyyy-MM-dd}.");

        if (await _citas.ExisteAsync(c => c.Id != citaIgnorada && c.PacienteId == pacienteId && c.OcupaHorario && c.SeCruzaCon(inicio, fin)))
            throw new ConflictoException($"El paciente ya tiene otra cita el {inicio:dd/MM/yyyy} a las {inicio:HH:mm}.");
    }

    private async Task<CitaResponseDto> ADtoAsync(Cita cita)
    {
        var paciente = await _pacientes.ObtenerPorIdAsync(cita.PacienteId);
        var medico = await _medicos.ObtenerPorIdAsync(cita.MedicoId);
        return cita.ADto(paciente!, medico!);
    }

    private async Task<IReadOnlyList<CitaResponseDto>> ADtosAsync(IEnumerable<Cita> citas)
    {
        var pacientes = (await _pacientes.ObtenerTodosAsync()).ToDictionary(p => p.Id);
        var medicos = (await _medicos.ObtenerTodosAsync()).ToDictionary(m => m.Id);
        return citas.Select(c => c.ADto(pacientes[c.PacienteId], medicos[c.MedicoId])).ToList();
    }
}
