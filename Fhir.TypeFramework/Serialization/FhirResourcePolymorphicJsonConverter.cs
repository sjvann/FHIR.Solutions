using System.Text.Json;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.Serialization;

/// <summary>
/// 針對抽象 <see cref="Resource"/> 依 JSON 的 <c>resourceType</c> 分派至具體子類型（可選、執行期註冊，不綁定特定 R5 組件）。
/// </summary>
public sealed class FhirResourcePolymorphicJsonConverter : JsonConverter<Resource>
{
    private readonly IReadOnlyDictionary<string, Type> _typesByName;
    private readonly JsonSerializerOptions _inner;

    public FhirResourcePolymorphicJsonConverter(
        IReadOnlyDictionary<string, Type> typesByName,
        JsonSerializerOptions template)
    {
        _typesByName = typesByName;
        _inner = new JsonSerializerOptions(template);
        for (var i = _inner.Converters.Count - 1; i >= 0; i--)
        {
            if (_inner.Converters[i] is FhirResourcePolymorphicJsonConverterFactory)
                _inner.Converters.RemoveAt(i);
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
        if (!_typesByName.TryGetValue(rt, out var concrete))
            throw new JsonException($"Unsupported FHIR resourceType \"{rt}\" for the registered type map.");

        return (Resource?)JsonSerializer.Deserialize(root.GetRawText(), concrete, _inner);
    }

    public override void Write(Utf8JsonWriter writer, Resource value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), _inner);
}

/// <summary>以自訂 <see cref="Resource"/> 多型別轉換器包裝 <see cref="FhirJsonSerializer.Options"/>。</summary>
public sealed class FhirResourcePolymorphicJsonConverterFactory : JsonConverterFactory
{
    private readonly IReadOnlyDictionary<string, Type> _typesByName;

    public FhirResourcePolymorphicJsonConverterFactory(IReadOnlyDictionary<string, Type> typesByName)
        => _typesByName = typesByName;

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Resource);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => new FhirResourcePolymorphicJsonConverter(_typesByName, options);
}
