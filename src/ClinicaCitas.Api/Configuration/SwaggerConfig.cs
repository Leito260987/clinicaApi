using System.Reflection;
using Microsoft.OpenApi.Models;

namespace ClinicaCitas.Api.Configuration;

/// <summary>Documentación interactiva de la API con Swagger (OpenAPI).</summary>
public static class SwaggerConfig
{
    public static IServiceCollection AddDocumentacionSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opciones =>
        {
            opciones.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Clínica – API de Citas Médicas",
                Version = "v1",
                Description = "API RESTful para registrar, consultar, reprogramar, cancelar y eliminar citas médicas. " +
                              "Los datos se guardan en memoria (List<T>) mientras la aplicación está en ejecución."
            });

            var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            if (File.Exists(xml)) opciones.IncludeXmlComments(xml);
        });
        return services;
    }

    public static WebApplication UseDocumentacionSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opciones =>
        {
            opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "Citas Médicas v1");
            opciones.DocumentTitle = "API de Citas Médicas";
        });
        return app;
    }
}
