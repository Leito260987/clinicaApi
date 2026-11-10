using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.Models;

/// <summary>
/// Cita médica. Las transiciones de estado se hacen solo con sus métodos,
/// para que nadie pueda, por ejemplo, reprogramar una cita cancelada.
/// </summary>
public class Cita : EntidadBase
{
    /// <summary>Duración fija de cada cita en minutos.</summary>
    public const int DuracionMinutos = 30;

    public int PacienteId { get; private set; }
    public int MedicoId { get; private set; }
    public DateTime FechaHora { get; private set; }
    public string Motivo { get; private set; }
    public EstadoCita Estado { get; private set; } = EstadoCita.Programada;
    public string? MotivoCancelacion { get; private set; }

    public DateTime FechaHoraFin => FechaHora.AddMinutes(DuracionMinutos);

    /// <summary>Indica si la cita ocupa el horario del médico y del paciente.</summary>
    public bool OcupaHorario => Estado != EstadoCita.Cancelada;

    public Cita(int pacienteId, int medicoId, DateTime fechaHora, string motivo)
    {
        PacienteId = pacienteId;
        MedicoId = medicoId;
        FechaHora = fechaHora;
        Motivo = motivo.Trim();
    }

    /// <summary>Cambia médico, fecha y motivo. Solo se permite si la cita está programada.</summary>
    public void Reprogramar(int medicoId, DateTime fechaHora, string motivo)
    {
        AsegurarQueEsteProgramada("reprogramar");
        MedicoId = medicoId;
        FechaHora = fechaHora;
        Motivo = motivo.Trim();
        MarcarActualizada();
    }

    /// <summary>Cancela la cita y libera el horario.</summary>
    public void Cancelar(string motivoCancelacion)
    {
        AsegurarQueEsteProgramada("cancelar");
        Estado = EstadoCita.Cancelada;
        MotivoCancelacion = motivoCancelacion.Trim();
        MarcarActualizada();
    }

    /// <summary>Registra que el paciente fue atendido.</summary>
    public void MarcarAtendida()
    {
        AsegurarQueEsteProgramada("marcar como atendida");
        Estado = EstadoCita.Atendida;
        MarcarActualizada();
    }

    /// <summary>Indica si esta cita se cruza con el intervalo indicado.</summary>
    public bool SeCruzaCon(DateTime inicio, DateTime fin) => FechaHora < fin && inicio < FechaHoraFin;

    /// <summary>Lanza una excepción de negocio si la cita ya no está programada.</summary>
    public void AsegurarQueEsteProgramada(string accion)
    {
        if (Estado != EstadoCita.Programada)
            throw new ReglaNegocioException(
                $"No se puede {accion} la cita {Id} porque está en estado '{Estado}'. Solo se modifican citas programadas.");
    }
}
