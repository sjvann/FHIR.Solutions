namespace Fhir.Terminology.Core;

public sealed record StoredResourceRow(
    Guid RowId,
    string LogicalId,
    string ResourceType,
    string FhirSpecVersion,
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

    /// <summary>FHIR 規格版本（例 <c>5.0.0</c>），對應資料庫 <c>FhirSpecVersion</c>。</summary>
    public string? FhirSpecVersion { get; init; }
}

public interface ITerminologyRepository
{
    Task<IReadOnlyList<StoredResourceRow>> SearchAsync(string resourceType, TerminologySearchParameters p, CancellationToken cancellationToken = default);

    Task<StoredResourceRow?> GetAsync(string resourceType, string logicalId, string fhirSpecVersion, CancellationToken cancellationToken = default);

    Task<StoredResourceRow> CreateAsync(string rawJson, string fhirSpecVersion, CancellationToken cancellationToken = default);

    /// <summary>依資源 <c>id</c> 與類型建立或更新（供 FHIR 套件批次匯入）。</summary>
    Task<StoredResourceRow> UpsertAsync(string rawJson, string fhirSpecVersion, CancellationToken cancellationToken = default);

    /// <summary>於單一交易中批次 upsert，回傳寫入筆數。</summary>
    Task<int> BulkUpsertAsync(IReadOnlyList<string> rawJsonResources, string fhirSpecVersion, CancellationToken cancellationToken = default);

    Task<StoredResourceRow?> UpdateAsync(string resourceType, string logicalId, string fhirSpecVersion, string rawJson, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string resourceType, string logicalId, string fhirSpecVersion, CancellationToken cancellationToken = default);

    /// <summary>依 canonical url（與選填 resource <paramref name="version"/>）於指定 FHIR 規格版本列中找第一筆資源。</summary>
    Task<StoredResourceRow?> FindByUrlAsync(string resourceType, string url, string? version, string fhirSpecVersion, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BindingRegistryRow>> ListBindingsAsync(CancellationToken cancellationToken = default);

    Task<BindingRegistryRow> AddBindingAsync(BindingRegistryRow row, CancellationToken cancellationToken = default);

    Task<bool> DeleteBindingAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除 <paramref name="structureDefinitionCanonicalUrl"/> 對應之全部 Binding 登錄列後，重新寫入（供自匯入之 StructureDefinition 同步）。
    /// </summary>
    Task ReplaceBindingsForStructureDefinitionAsync(string structureDefinitionCanonicalUrl, IReadOnlyList<BindingRegistryRow> rows, CancellationToken cancellationToken = default);

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
