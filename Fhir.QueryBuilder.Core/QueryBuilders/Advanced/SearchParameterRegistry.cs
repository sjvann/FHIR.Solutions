using Fhir.QueryBuilder.Common;

namespace Fhir.QueryBuilder.QueryBuilders.Advanced;

public interface ISearchParameterRegistry
{
    Task<RegistrySearchParameterDefinition?> GetSearchParameterAsync(string resourceType, string name, FhirVersion version);

    Task<IReadOnlyList<string>> GetParameterModifiersAsync(string resourceType, string name, FhirVersion version);
}

public sealed class RegistrySearchParameterDefinition
{
    public string Name { get; set; } = string.Empty;

    public SearchParameterType Type { get; set; }
}

/// <summary>預設寬鬆註冊表（不阻擋查詢建構；進階驗證可日後改為 Capability 驅動）。</summary>
public sealed class PermissiveSearchParameterRegistry : ISearchParameterRegistry
{
    public Task<RegistrySearchParameterDefinition?> GetSearchParameterAsync(string resourceType, string name,
        FhirVersion version)
    {
        return Task.FromResult<RegistrySearchParameterDefinition?>(new RegistrySearchParameterDefinition
        {
            Name = name,
            Type = SearchParameterType.String
        });
    }

    public Task<IReadOnlyList<string>> GetParameterModifiersAsync(string resourceType, string name, FhirVersion version)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
}
