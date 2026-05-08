using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Serialization;

namespace Fhir.Resources.R5.Serialization;

/// <summary>
/// FHIR R5 資源 JSON：依 <c>resourceType</c> 反序列化為具體 POCO（例如 <see cref="Bundle"/> 內 <c>entry.resource</c>）。
/// </summary>
public static class FhirR5Json
{
    private static readonly Lazy<IReadOnlyDictionary<string, Type>> ResourceTypesByName =
        new(BuildResourceTypeMap);

    private static readonly Lazy<JsonSerializerOptions> OptionsWithResourceConverter = new(CreateOptions);

    /// <summary>
    /// 與 <see cref="FhirJsonSerializer.Options"/> 相同設定，並附加 Resource 多型別 converter。
    /// </summary>
    public static JsonSerializerOptions SerializerOptions => OptionsWithResourceConverter.Value;

    /// <summary>
    /// 將任意資源 JSON 解析為具體 <see cref="Resource"/> 子類型。
    /// </summary>
    public static Resource? ParseResource(string json)
        => JsonSerializer.Deserialize<Resource>(json, SerializerOptions);

    /// <summary>
    /// 將 Bundle JSON 解析為強型別（含 entry.resource 多型別）。
    /// </summary>
    public static Bundle? ParseBundle(string json)
        => JsonSerializer.Deserialize<Bundle>(json, SerializerOptions);

    private static JsonSerializerOptions CreateOptions()
    {
        var o = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
        foreach (var c in FhirJsonSerializer.Options.Converters)
            o.Converters.Add(c);
        o.Converters.Add(new FhirR5ResourceJsonConverterFactory());
        return o;
    }

    private static IReadOnlyDictionary<string, Type> BuildResourceTypeMap()
    {
        var dict = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (var type in typeof(Bundle).Assembly.GetTypes())
        {
            if (type.IsAbstract || !typeof(Resource).IsAssignableFrom(type))
                continue;
            var field = type.GetField("ResourceTypeValue", BindingFlags.Public | BindingFlags.Static);
            if (field?.GetValue(null) is string name && !string.IsNullOrEmpty(name))
                dict[name] = type;
        }

        return dict;
    }

    internal static bool TryGetConcreteResourceType(string resourceType, [NotNullWhen(true)] out Type? type)
        => ResourceTypesByName.Value.TryGetValue(resourceType, out type);

    private sealed class FhirR5ResourceJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Resource);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            => new FhirR5ResourceJsonConverter(options);
    }

    private sealed class FhirR5ResourceJsonConverter : JsonConverter<Resource>
    {
        private readonly JsonSerializerOptions _optionsWithoutResourceConverter;

        public FhirR5ResourceJsonConverter(JsonSerializerOptions template)
        {
            _optionsWithoutResourceConverter = new JsonSerializerOptions(template);
            for (var i = _optionsWithoutResourceConverter.Converters.Count - 1; i >= 0; i--)
            {
                if (_optionsWithoutResourceConverter.Converters[i] is FhirR5ResourceJsonConverterFactory)
                    _optionsWithoutResourceConverter.Converters.RemoveAt(i);
            }
        }

        public override Resource? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            if (!root.TryGetProperty("resourceType", out var rtEl))
                throw new JsonException("FHIR JSON object is missing \"resourceType\".");
            var rt = rtEl.GetString();
            if (string.IsNullOrEmpty(rt))
                throw new JsonException("FHIR \"resourceType\" is empty.");
            if (!TryGetConcreteResourceType(rt, out var concrete))
                throw new JsonException($"Unsupported FHIR resourceType \"{rt}\" for this assembly.");

            return (Resource?)JsonSerializer.Deserialize(root.GetRawText(), concrete, _optionsWithoutResourceConverter);
        }

        public override void Write(Utf8JsonWriter writer, Resource value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), _optionsWithoutResourceConverter);
    }
}
