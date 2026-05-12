namespace Fhir.VersionManager.Capability;

/// <summary>CapabilityStatement 內單一 rest.resource 之搜尋參數（跨線別統一形狀）。</summary>
public sealed class CapabilitySearchParamModel
{
    public string? Name { get; init; }
    public string? Type { get; init; }
    public string? Documentation { get; init; }
}

/// <summary>CapabilityStatement 內單一 rest.resource（跨線別統一形狀）。</summary>
public sealed class CapabilityRestResourceModel
{
    public string? Type { get; init; }
    public IReadOnlyList<string> InteractionCodes { get; init; } = Array.Empty<string>();
    public IReadOnlyList<CapabilitySearchParamModel> SearchParams { get; init; } = Array.Empty<CapabilitySearchParamModel>();
    public IReadOnlyList<string> SearchIncludes { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> SearchRevIncludes { get; init; } = Array.Empty<string>();
}

/// <summary>窄化 CapabilityStatement：供 UI／查詢建構讀取，不依賴單一線別 POCO。</summary>
public interface ICapabilityModel
{
    FhirVersion Version { get; }
    string? FhirVersionElement { get; }
    string? SoftwareName { get; }
    IReadOnlyList<string> ImplementationGuideUrls { get; }
    IReadOnlyList<string> Formats { get; }
    IReadOnlyList<CapabilityRestResourceModel> ServerResources { get; }
}

internal sealed class CapabilityModel : ICapabilityModel
{
    public required FhirVersion Version { get; init; }
    public string? FhirVersionElement { get; init; }
    public string? SoftwareName { get; init; }
    public IReadOnlyList<string> ImplementationGuideUrls { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Formats { get; init; } = Array.Empty<string>();
    public IReadOnlyList<CapabilityRestResourceModel> ServerResources { get; init; } = Array.Empty<CapabilityRestResourceModel>();
}
