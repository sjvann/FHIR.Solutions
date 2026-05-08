using System.Text.Json.Serialization;

namespace FhirResourceCreator.StructureDefinition;

public sealed class StructureDefinitionDocument
{
    [JsonPropertyName("resourceType")]
    public string? ResourceType { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("kind")]
    public string? Kind { get; set; }

    [JsonPropertyName("abstract")]
    public bool? Abstract { get; set; }

    [JsonPropertyName("snapshot")]
    public SnapshotDefinition? Snapshot { get; set; }
}

public sealed class SnapshotDefinition
{
    [JsonPropertyName("element")]
    public List<ElementDefinitionModel>? Element { get; set; }
}

public sealed class ElementDefinitionModel
{
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("min")]
    public int Min { get; set; }

    [JsonPropertyName("max")]
    public string? Max { get; set; }

    [JsonPropertyName("type")]
    public List<ElementTypeRef>? Type { get; set; }

    [JsonPropertyName("contentReference")]
    public string? ContentReference { get; set; }
}

public sealed class ElementTypeRef
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("targetProfile")]
    public List<string>? TargetProfile { get; set; }
}
