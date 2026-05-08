using Fhir.Resources.R5;
using Fhir.Terminology.Core;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fhir.Terminology.Infrastructure;

public class EfTerminologyRepository(TerminologyDbContext db, ILogger<EfTerminologyRepository> logger) : ITerminologyRepository
{
    private readonly TerminologyDbContext _db = db;
    private readonly ILogger<EfTerminologyRepository> _logger = logger;

    public async Task<IReadOnlyList<StoredResourceRow>> SearchAsync(string resourceType, TerminologySearchParameters p, CancellationToken cancellationToken = default)
    {
        var q = _db.TerminologyResources.AsNoTracking().Where(x => x.ResourceType == resourceType);
        if (!string.IsNullOrEmpty(p.Url))
            q = q.Where(x => x.Url != null && x.Url == p.Url);
        if (!string.IsNullOrEmpty(p.Version))
            q = q.Where(x => x.Version != null && x.Version == p.Version);
        if (!string.IsNullOrEmpty(p.Name))
            q = q.Where(x => x.Name != null && x.Name == p.Name);
        if (!string.IsNullOrEmpty(p.Title))
            q = q.Where(x => x.Title != null && x.Title == p.Title);
        if (!string.IsNullOrEmpty(p.Status))
            q = q.Where(x => x.Status != null && x.Status == p.Status);

        var list = await q.ToListAsync(cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<StoredResourceRow?> GetAsync(string resourceType, string logicalId, CancellationToken cancellationToken = default)
    {
        var e = await _db.TerminologyResources.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ResourceType == resourceType && x.LogicalId == logicalId, cancellationToken);
        return e is null ? null : Map(e);
    }

    public async Task<StoredResourceRow> CreateAsync(string rawJson, CancellationToken cancellationToken = default)
    {
        var resource = TerminologyJson.DeserializeResource(rawJson);
        if (resource is not DomainResource dr)
            throw new ArgumentException("Expected DomainResource");

        var rt = GetResourceType(dr);
        var id = dr.Id?.StringValue ?? Guid.NewGuid().ToString("n");
        dr.Id = new FhirId(id);
        rawJson = TerminologyJson.Serialize(dr);

        var meta = ExtractIndex(dr, rt);
        var entity = new TerminologyResourceEntity
        {
            Id = Guid.NewGuid(),
            ResourceType = rt,
            LogicalId = id,
            RawJson = rawJson,
            Url = meta.url,
            Version = meta.version,
            Name = meta.name,
            Title = meta.title,
            Status = meta.status,
        };

        _db.TerminologyResources.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<StoredResourceRow?> UpdateAsync(string resourceType, string logicalId, string rawJson, CancellationToken cancellationToken = default)
    {
        var entity = await _db.TerminologyResources.FirstOrDefaultAsync(
            x => x.ResourceType == resourceType && x.LogicalId == logicalId, cancellationToken);
        if (entity is null)
            return null;

        var resource = TerminologyJson.DeserializeResource(rawJson);
        if (resource is not DomainResource dr)
            throw new ArgumentException("Expected DomainResource");

        dr.Id = new FhirId(logicalId);
        rawJson = TerminologyJson.Serialize(dr);

        var meta = ExtractIndex(dr, resourceType);
        entity.RawJson = rawJson;
        entity.Url = meta.url;
        entity.Version = meta.version;
        entity.Name = meta.name;
        entity.Title = meta.title;
        entity.Status = meta.status;

        await _db.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteAsync(string resourceType, string logicalId, CancellationToken cancellationToken = default)
    {
        var entity = await _db.TerminologyResources.FirstOrDefaultAsync(
            x => x.ResourceType == resourceType && x.LogicalId == logicalId, cancellationToken);
        if (entity is null)
            return false;

        _db.TerminologyResources.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<StoredResourceRow?> FindByUrlAsync(string resourceType, string url, string? version, CancellationToken cancellationToken = default)
    {
        var q = _db.TerminologyResources.AsNoTracking().Where(x => x.ResourceType == resourceType && x.Url == url);
        if (!string.IsNullOrEmpty(version))
            q = q.Where(x => x.Version == version);

        var match = await q.OrderByDescending(x => x.Version).FirstOrDefaultAsync(cancellationToken);
        return match is null ? null : Map(match);
    }

    public async Task<IReadOnlyList<BindingRegistryRow>> ListBindingsAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.BindingRegistry.AsNoTracking().ToListAsync(cancellationToken);
        return rows.Select(x => new BindingRegistryRow(x.Id, x.StructureDefinitionUrl, x.ElementPath, x.ValueSetCanonical, x.Strength)).ToList();
    }

    public async Task<BindingRegistryRow> AddBindingAsync(BindingRegistryRow row, CancellationToken cancellationToken = default)
    {
        var e = new BindingRegistryEntity
        {
            Id = row.Id == Guid.Empty ? Guid.NewGuid() : row.Id,
            StructureDefinitionUrl = row.StructureDefinitionUrl,
            ElementPath = row.ElementPath,
            ValueSetCanonical = row.ValueSetCanonical,
            Strength = row.Strength,
        };
        _db.BindingRegistry.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        return new BindingRegistryRow(e.Id, e.StructureDefinitionUrl, e.ElementPath, e.ValueSetCanonical, e.Strength);
    }

    public async Task<bool> DeleteBindingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.BindingRegistry.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (e is null)
            return false;

        _db.BindingRegistry.Remove(e);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<UpstreamServerRow>> ListUpstreamsAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.UpstreamServers.AsNoTracking().ToListAsync(cancellationToken);
        return rows.Select(x => new UpstreamServerRow(x.Id, x.SystemUriPrefix, x.BaseUrl, x.AuthorizationHeader, x.TimeoutSeconds)).ToList();
    }

    public async Task<UpstreamServerRow> AddUpstreamAsync(UpstreamServerRow row, CancellationToken cancellationToken = default)
    {
        var e = new UpstreamServerEntity
        {
            Id = row.Id == Guid.Empty ? Guid.NewGuid() : row.Id,
            SystemUriPrefix = row.SystemUriPrefix,
            BaseUrl = row.BaseUrl.TrimEnd('/'),
            AuthorizationHeader = row.AuthorizationHeader,
            TimeoutSeconds = row.TimeoutSeconds,
        };
        _db.UpstreamServers.Add(e);
        await _db.SaveChangesAsync(cancellationToken);
        return new UpstreamServerRow(e.Id, e.SystemUriPrefix, e.BaseUrl, e.AuthorizationHeader, e.TimeoutSeconds);
    }

    public async Task<bool> DeleteUpstreamAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var e = await _db.UpstreamServers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (e is null)
            return false;

        _db.UpstreamServers.Remove(e);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static StoredResourceRow Map(TerminologyResourceEntity e) =>
        new(e.Id, e.LogicalId, e.ResourceType, e.RawJson, e.Url, e.Version, e.Name, e.Title, e.Status);

    private static string GetResourceType(DomainResource r) => r switch
    {
        CodeSystem => CodeSystem.ResourceTypeValue,
        ValueSet => ValueSet.ResourceTypeValue,
        ConceptMap => ConceptMap.ResourceTypeValue,
        _ => "",
    };

    private static (string? url, string? version, string? name, string? title, string? status) ExtractIndex(DomainResource dr, string resourceType)
    {
        return resourceType switch
        {
            CodeSystem.ResourceTypeValue when dr is CodeSystem cs => (
                cs.Url?.StringValue,
                cs.Version?.StringValue,
                cs.Name?.StringValue,
                cs.Title?.StringValue,
                cs.Status?.StringValue),
            ValueSet.ResourceTypeValue when dr is ValueSet vs => (
                vs.Url?.StringValue,
                vs.Version?.StringValue,
                vs.Name?.StringValue,
                vs.Title?.StringValue,
                vs.Status?.StringValue),
            ConceptMap.ResourceTypeValue when dr is ConceptMap cm => (
                cm.Url?.StringValue,
                cm.Version?.StringValue,
                cm.Name?.StringValue,
                cm.Title?.StringValue,
                cm.Status?.StringValue),
            _ => (null, null, null, null, null),
        };
    }
}
