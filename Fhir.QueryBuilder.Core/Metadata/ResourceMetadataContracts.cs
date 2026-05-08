using Fhir.QueryBuilder.Common;

namespace Fhir.QueryBuilder.Metadata;

/// <summary>依 CapabilityStatement 提供的資源／搜尋參數 metadata。</summary>
public interface IMetadataResourceProvider
{
    FhirVersion SupportedVersion { get; }

    bool IsSupported(string resourceType);

    ResourceTypeMetadata? GetResourceMetadata(string resourceType);
}

public sealed class ResourceTypeMetadata
{
    public string ResourceType { get; set; } = string.Empty;

    public List<SearchParameterMetadataEntry> SearchParameters { get; set; } = new();
}

public sealed class SearchParameterMetadataEntry
{
    public string Name { get; set; } = string.Empty;

    public SearchParameterType Type { get; set; }

    /// <summary>若伺服器未宣告修飾符，則為空（驗證略過修飾符檢查）。</summary>
    public List<string> SupportedModifiers { get; set; } = new();
}
