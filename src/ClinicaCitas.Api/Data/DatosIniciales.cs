using ClinicaCitas.Api.Models;
using ClinicaCitas.Api.Models.Enums;
using ClinicaCitas.Api.Repositories.Interfaces;

namespace ClinicaCitas.Api.Data;

/// <summary>
/// Carga datos de ejemplo al iniciar la API para poder probarla de inmediato.
/// Las citas se generan en los próximos días hábiles, así siempre son futuras.
/// </summary>
public static class DatosIniciales
{
    public static async Task CargarAsync(IServiceProvider servicios)
    {
        var pacientes = servicios.GetRequiredService<IRepository<Paciente>>();
        var medicos = servicios.GetRequiredService<IRepository<Medico>>();
        var citas = servicios.GetRequiredService<IRepository<Cita>>();

        if ((await pacientes.ObtenerTodosAsync()).Count > 0) return;

        var ana = await pacientes.AgregarAsync(new Paciente("Ana Lucía", "Torres Vega", "45879632", "ana.torres@correo.com", "987654321", new DateTime(1990, 5, 14)));
        var carlos = await pacientes.AgregarAsync(new Paciente("Carlos Andrés", "Quispe Rojas", "47125836", "carlos.quispe@correo.com", "912345678", new DateTime(1985, 11, 2)));
        await pacientes.AgregarAsync(new Paciente("Rosa María", "Díaz Paredes", "41236587", "rosa.diaz@correo.com", "923456789", new DateTime(2001, 3, 27)));

        var cardiologo = await medicos.AgregarAsync(new Medico("Jorge Luis", "Ramírez Soto", "40125896", "jramirez@clinica.pe", "934567890", "045678", Especialidad.Cardiologia));
        var pediatra = await medicos.AgregarAsync(new Medico("Patricia", "Huamán León", "42589631", "phuaman@clinica.pe", "945678901", "052341", Especialidad.Pediatria));
        await medicos.AgregarAsync(new Medico("Miguel Ángel", "Flores Cárdenas", "43698521", "mflores@clinica.pe", "956789012", "061245", Especialidad.MedicinaGeneral));

        var dia1 = ProximoDiaHabil(1);
        var dia2 = ProximoDiaHabil(2);
        await citas.AgregarAsync(new Cita(ana.Id, cardiologo.Id, dia1.AddHours(9), "Control de presión arterial"));
        await citas.AgregarAsync(new Cita(carlos.Id, cardiologo.Id, dia1.AddHours(9.5), "Dolor en el pecho al hacer ejercicio"));
        var cancelada = await citas.AgregarAsync(new Cita(carlos.Id, pediatra.Id, dia2.AddHours(15), "Control de crecimiento de su hijo"));
        cancelada.Cancelar("El paciente reprogramará la próxima semana");
        await citas.ActualizarAsync(cancelada);
    }

    /// <summary>Fecha (00:00) del n-ésimo día hábil a partir de mañana, saltando domingos.</summary>
    private static DateTime ProximoDiaHabil(int n)
    {
        var dia = DateTime.Today;
        while (n > 0)
        {
            dia = dia.AddDays(1);
            if (dia.DayOfWeek != DayOfWeek.Sunday) n--;
        }
        return dia;
    }
}
