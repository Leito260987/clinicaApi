namespace ClinicaCitas.Api.Models.Enums;

/// <summary>Ciclo de vida de una cita médica.</summary>
public enum EstadoCita
{
    /// <summary>Cita registrada y pendiente de atención.</summary>
    Programada = 1,
    /// <summary>Cita cancelada por el paciente o la clínica.</summary>
    Cancelada = 2,
    /// <summary>Cita en la que el paciente ya fue atendido.</summary>
    Atendida = 3
}
