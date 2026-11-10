using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Models;

namespace ClinicaCitas.Api.Services;

/// <summary>
/// Reglas del horario de la clínica: lunes a sábado, de 08:00 a 20:00,
/// en bloques de 30 minutos (la última cita empieza a las 19:30).
/// </summary>
public static class HorarioAtencion
{
    public static readonly TimeOnly Apertura = new(8, 0);
    public static readonly TimeOnly Cierre = new(20, 0);

    /// <summary>Lanza una excepción si la fecha no cae en un bloque válido del horario.</summary>
    public static void Validar(DateTime fechaHora)
    {
        if (fechaHora.DayOfWeek == DayOfWeek.Sunday)
            throw new ReglaNegocioException("La clínica no atiende los domingos. Elija un día de lunes a sábado.");

        var hora = TimeOnly.FromDateTime(fechaHora);
        var fin = hora.AddMinutes(Cita.DuracionMinutos);
        if (hora < Apertura || fin > Cierre || fin < hora)
            throw new ReglaNegocioException("El horario de atención es de 08:00 a 20:00; la última cita empieza a las 19:30.");

        if (fechaHora.Minute % Cita.DuracionMinutos != 0 || fechaHora.Second != 0)
            throw new ReglaNegocioException("Las citas se programan en bloques de 30 minutos (por ejemplo 10:00 o 10:30).");
    }

    /// <summary>Todos los bloques de inicio de un día de atención.</summary>
    public static IEnumerable<DateTime> BloquesDelDia(DateOnly fecha)
    {
        if (fecha.DayOfWeek == DayOfWeek.Sunday) yield break;
        for (var hora = Apertura; hora.AddMinutes(Cita.DuracionMinutos) <= Cierre && hora >= Apertura;
             hora = hora.AddMinutes(Cita.DuracionMinutos))
            yield return fecha.ToDateTime(hora);
    }
}
