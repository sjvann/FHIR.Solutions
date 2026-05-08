using System.Collections.Frozen;
using System.Text.Json;
using Fhir.Resources.R5;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Serialization;

namespace Fhir.Terminology.Core;

/// <summary>術語服務使用的 FHIR JSON 選項（含資源多型別）。</summary>
public static class TerminologyJson
{
    private static readonly Lazy<JsonSerializerOptions> OptionsLazy = new(CreateOptions);

    public static JsonSerializerOptions SerializerOptions => OptionsLazy.Value;

    public static string Serialize<T>(T resource) where T : Base
        // 使用執行時型別，避免從 API 以 Base/Resource 傳入時誤用 abstract 的 typeof(T) 而丟失欄位（如 OperationOutcome.issue）。
        => JsonSerializer.Serialize(resource, resource.GetType(), SerializerOptions);

    public static T? Deserialize<T>(string json) where T : Base
        => JsonSerializer.Deserialize<T>(json, SerializerOptions);

    public static Resource? DeserializeResource(string json)
        => JsonSerializer.Deserialize<Resource>(json, SerializerOptions);

    private static JsonSerializerOptions CreateOptions()
    {
        var map = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            ["CodeSystem"] = typeof(CodeSystem),
            ["ValueSet"] = typeof(ValueSet),
            ["ConceptMap"] = typeof(ConceptMap),
            ["Bundle"] = typeof(Bundle),
            ["OperationOutcome"] = typeof(OperationOutcome),
            ["Parameters"] = typeof(Parameters),
            ["CapabilityStatement"] = typeof(CapabilityStatement),
            ["TerminologyCapabilities"] = typeof(TerminologyCapabilities),
        };

        return FhirJsonSerializer.OptionsWithPolymorphicResources(map.ToFrozenDictionary(StringComparer.Ordinal));
    }
}
