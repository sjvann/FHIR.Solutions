using Fhir.QueryBuilder.Common;
using Fhir.VersionManager;
using Fhir.VersionManager.Capability;

namespace Fhir.QueryBuilder.Services.Interfaces;

/// <summary>伺服器相容性服務介面</summary>
public interface IServerCompatibilityService
{
    Task<ServerCompatibilityReport> CheckCompatibilityAsync(string serverUrl);

    Task<ResourceSupportReport> CheckResourceSupportAsync(string serverUrl, string resourceType);

    Task<SearchParameterSupportReport> CheckSearchParameterSupportAsync(
        string serverUrl, string resourceType, string searchParameter);

    Task<OperationSupportReport> CheckOperationSupportAsync(string serverUrl, string operation);

    Task<CapabilityParseResult?> GetCapabilityStatementAsync(string serverUrl);

    Task<FhirVersion?> DetectFhirVersionAsync(string serverUrl);

    Task ClearCacheAsync(string? serverUrl = null);
}

public class ServerCompatibilityReport
{
    public string ServerUrl { get; set; } = string.Empty;
    public FhirVersion? DetectedVersion { get; set; }
    public string? ServerSoftware { get; set; }
    public List<string> ImplementationGuides { get; set; } = new();
    public List<ResourceSupportInfo> ResourceSupport { get; set; } = new();
    public List<string> SupportedOperations { get; set; } = new();
    public int OverallCompatibilityScore { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public bool IsAvailable { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ResourceSupportReport
{
    public string ResourceType { get; set; } = string.Empty;
    public bool IsSupported { get; set; }
    public List<string> SupportedInteractions { get; set; } = new();
    public List<SearchParameterSupportInfo> SearchParameters { get; set; } = new();
    public string? Versioning { get; set; }
    public bool ConditionalCreate { get; set; }
    public bool ConditionalUpdate { get; set; }
    public bool ConditionalDelete { get; set; }
    public List<string> SupportedIncludes { get; set; } = new();
    public List<string> SupportedRevIncludes { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class SearchParameterSupportReport
{
    public string ParameterName { get; set; } = string.Empty;
    public bool IsSupported { get; set; }
    public SearchParameterType Type { get; set; }
    public List<string> SupportedModifiers { get; set; } = new();
    public string? Documentation { get; set; }
    public string? ErrorMessage { get; set; }
}

public class OperationSupportReport
{
    public string OperationName { get; set; } = string.Empty;
    public bool IsSupported { get; set; }
    public string? DefinitionUrl { get; set; }
    public string? Documentation { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ResourceSupportInfo
{
    public string Type { get; set; } = string.Empty;
    public List<string> Interactions { get; set; } = new();
    public int SearchParameterCount { get; set; }
    public int CompatibilityScore { get; set; }
}

public class SearchParameterSupportInfo
{
    public string Name { get; set; } = string.Empty;
    public SearchParameterType Type { get; set; }
    public List<string> Modifiers { get; set; } = new();
    public bool IsStandard { get; set; }
}
