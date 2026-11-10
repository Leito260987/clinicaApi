using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClinicaCitas.Api.Configuration;

/// <summary>
/// Las citas se manejan en hora local de la clínica. Este convertidor lee y escribe
/// fechas como "yyyy-MM-ddTHH:mm:ss", sin zona horaria, para que el cliente vea
/// exactamente la hora que registró.
/// </summary>
public class FechaHoraLocalConverter : JsonConverter<DateTime>
{
    private const string Formato = "yyyy-MM-dd'T'HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var texto = reader.GetString();
        if (!DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var fecha))
            throw new JsonException($"'{texto}' no es una fecha válida. Use el formato yyyy-MM-ddTHH:mm:ss.");
        if (fecha.Kind == DateTimeKind.Utc) fecha = fecha.ToLocalTime();
        return DateTime.SpecifyKind(fecha, DateTimeKind.Unspecified);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString(Formato, CultureInfo.InvariantCulture));
}
