namespace ClinicaCitas.Api.Models;

/// <summary>Paciente que solicita citas en la clínica.</summary>
public class Paciente : Persona
{
    public DateTime FechaNacimiento { get; private set; }

    /// <summary>Edad calculada a la fecha actual.</summary>
    public int Edad
    {
        get
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - FechaNacimiento.Year;
            if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }
    }

    public Paciente(string nombres, string apellidos, string dni, string email, string telefono, DateTime fechaNacimiento)
        : base(nombres, apellidos, dni, email, telefono)
    {
        FechaNacimiento = fechaNacimiento.Date;
    }

    public void Actualizar(string nombres, string apellidos, string dni, string email, string telefono, DateTime fechaNacimiento)
    {
        FechaNacimiento = fechaNacimiento.Date;
        ActualizarDatosPersonales(nombres, apellidos, dni, email, telefono);
    }
}
