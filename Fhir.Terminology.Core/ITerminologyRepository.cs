namespace Fhir.Terminology.Core;

public sealed record StoredResourceRow(
    Guid RowId,
    string LogicalId,
    string ResourceType,
    string RawJson,
    string? Url,
    string? Version,
    string? Name,
    string? Title,
    string? Status);

public sealed class TerminologySearchParameters
{
    public string? Url { get; init; }
    public string? Version { get; init; }
    public string? Name { get; init; }
    public string? Title { get; init; }
    public string? Status { get; init; }
}

public interface ITerminologyRepository
{
    Task<IReadOnlyList<StoredResourceRow>> SearchAsync(string resourceType, TerminologySearchParameters p, CancellationToken cancellationToken = default);

    Task<StoredResourceRow?> GetAsync(string resourceType, string logicalId, CancellationToken cancellationToken = default);

    Task<StoredResourceRow> CreateAsync(string rawJson, CancellationToken cancellationToken = default);

    Task<StoredResourceRow?> UpdateAsync(string resourceType, string logicalId, string rawJson, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string resourceType, string logicalId, CancellationToken cancellationToken = default);

    /// <summary>依 canonical url（與選填 version）找第一筆資源。</summary>
    Task<StoredResourceRow?> FindByUrlAsync(string resourceType, string url, string? version, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BindingRegistryRow>> ListBindingsAsync(CancellationToken cancellationToken = default);

    Task<BindingRegistryRow> AddBindingAsync(BindingRegistryRow row, CancellationToken cancellationToken = default);

    Task<bool> DeleteBindingAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UpstreamServerRow>> ListUpstreamsAsync(CancellationToken cancellationToken = default);

    Task<UpstreamServerRow> AddUpstreamAsync(UpstreamServerRow row, CancellationToken cancellationToken = default);

    Task<bool> DeleteUpstreamAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed record BindingRegistryRow(
    Guid Id,
    string? StructureDefinitionUrl,
    string? ElementPath,
    string ValueSetCanonical,
    string Strength);

public sealed record UpstreamServerRow(
    Guid Id,
    string SystemUriPrefix,
    string BaseUrl,
    string? AuthorizationHeader,
    int TimeoutSeconds);
