using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.Models;

/// <summary>Médico que atiende citas.</summary>
public class Medico : Persona
{
    /// <summary>Número de colegiatura del Colegio Médico del Perú (CMP).</summary>
    public string Cmp { get; private set; }

    public Especialidad Especialidad { get; private set; }

    /// <summary>Solo los médicos activos pueden recibir nuevas citas.</summary>
    public bool Activo { get; private set; } = true;

    public Medico(string nombres, string apellidos, string dni, string email, string telefono, string cmp, Especialidad especialidad)
        : base(nombres, apellidos, dni, email, telefono)
    {
        Cmp = cmp.Trim();
        Especialidad = especialidad;
    }

    public void Actualizar(string nombres, string apellidos, string dni, string email, string telefono,
        string cmp, Especialidad especialidad, bool activo)
    {
        Cmp = cmp.Trim();
        Especialidad = especialidad;
        Activo = activo;
        ActualizarDatosPersonales(nombres, apellidos, dni, email, telefono);
    }
}
