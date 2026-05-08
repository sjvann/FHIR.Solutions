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

    /// <summary>
    /// 複製預設 FHIR JSON 選項並附加依 <paramref name="resourceTypes"/> 分派的 <see cref="Resource"/> 多型別反序列化。
    /// </summary>
    public static JsonSerializerOptions OptionsWithPolymorphicResources(IReadOnlyDictionary<string, Type> resourceTypes)
    {
        var options = new JsonSerializerOptions(_options);
        options.Converters.Add(new FhirResourcePolymorphicJsonConverterFactory(resourceTypes));
        return options;
    }

    /// <summary>
    /// 將任意資源 JSON（須含 resourceType）解析為具體 <see cref="Resource"/> 子類別。
    /// </summary>
    public static Resource? DeserializeResource(string json, IReadOnlyDictionary<string, Type> resourceTypes)
        => JsonSerializer.Deserialize<Resource>(json, OptionsWithPolymorphicResources(resourceTypes));

    private static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        options.Converters.Add(new FhirPrimitiveJsonConverterFactory());
        options.Converters.Add(new FhirExtensionListJsonConverterFactory());

        return options;
    }
}

