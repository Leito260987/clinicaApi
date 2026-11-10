using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.DTOs.Medicos;
using ClinicaCitas.Api.DTOs.Pacientes;
using ClinicaCitas.Api.Models;

namespace ClinicaCitas.Api.Mappers;

/// <summary>Convierte entidades del modelo en DTOs de respuesta (la API nunca expone las entidades).</summary>
public static class MapeoExtensions
{
    public static PacienteResponseDto ADto(this Paciente p) => new()
    {
        Id = p.Id,
        Nombres = p.Nombres,
        Apellidos = p.Apellidos,
        NombreCompleto = p.NombreCompleto,
        Dni = p.Dni,
        Email = p.Email,
        Telefono = p.Telefono,
        FechaNacimiento = p.FechaNacimiento,
        Edad = p.Edad,
        FechaRegistro = p.FechaRegistro,
        FechaActualizacion = p.FechaActualizacion
    };

    public static MedicoResponseDto ADto(this Medico m) => new()
    {
        Id = m.Id,
        Nombres = m.Nombres,
        Apellidos = m.Apellidos,
        NombreCompleto = m.NombreCompleto,
        Dni = m.Dni,
        Email = m.Email,
        Telefono = m.Telefono,
        Cmp = m.Cmp,
        Especialidad = m.Especialidad.ToString(),
        Activo = m.Activo,
        FechaRegistro = m.FechaRegistro,
        FechaActualizacion = m.FechaActualizacion
    };

    public static CitaResponseDto ADto(this Cita c, Paciente paciente, Medico medico) => new()
    {
        Id = c.Id,
        FechaHora = c.FechaHora,
        FechaHoraFin = c.FechaHoraFin,
        Motivo = c.Motivo,
        Estado = c.Estado.ToString(),
        MotivoCancelacion = c.MotivoCancelacion,
        Paciente = new PersonaResumenDto { Id = paciente.Id, NombreCompleto = paciente.NombreCompleto, Dni = paciente.Dni },
        Medico = new MedicoResumenDto { Id = medico.Id, NombreCompleto = medico.NombreCompleto, Especialidad = medico.Especialidad.ToString() },
        FechaRegistro = c.FechaRegistro,
        FechaActualizacion = c.FechaActualizacion
    };
}
