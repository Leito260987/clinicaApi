using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using ClinicaCitas.Api.Configuration;
using ClinicaCitas.Api.Data;
using ClinicaCitas.Api.Middleware;
using ClinicaCitas.Api.Repositories;
using ClinicaCitas.Api.Repositories.Interfaces;
using ClinicaCitas.Api.Services;
using ClinicaCitas.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Controladores, JSON y validación
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());   // enums como texto: "Programada"
        o.JsonSerializerOptions.Converters.Add(new FechaHoraLocalConverter());   // fechas sin zona horaria
        o.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping; // tildes y ñ legibles
    })
    .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = RespuestasDeValidacion.Crear);
builder.Services.AddRouting(o => o.LowercaseUrls = true);

// ---------------- Persistencia simulada: un repositorio en memoria por entidad (Singleton = vive mientras corre la API)
builder.Services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));

// ---------------- Lógica de negocio (inyección de dependencias por interfaz)
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<ICitaService, CitaService>();

// ---------------- Documentación
builder.Services.AddDocumentacionSwagger();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(RespuestasDeEstado.EscribirAsync);   // 404 de rutas inexistentes, 405, etc. con el mismo formato
app.UseDocumentacionSwagger();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

await DatosIniciales.CargarAsync(app.Services);

app.Run();

/// <summary>Expuesto para pruebas de integración.</summary>
public partial class Program { }
