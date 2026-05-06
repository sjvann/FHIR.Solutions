using System.Text.Json;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.Serialization;

public static class FhirJsonSerializer
{
    private static readonly JsonSerializerOptions _options = CreateDefaultOptions();

    public static JsonSerializerOptions Options => _options;

    public static string Serialize(Base instance)
        => JsonSerializer.Serialize(instance, instance.GetType(), _options);

    public static string Serialize<T>(T instance) where T : Base
        => JsonSerializer.Serialize(instance, _options);

    public static T? Deserialize<T>(string json) where T : Base
        => JsonSerializer.Deserialize<T>(json, _options);

    private static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        options.Converters.Add(new FhirPrimitiveJsonConverterFactory());

        return options;
    }
}

