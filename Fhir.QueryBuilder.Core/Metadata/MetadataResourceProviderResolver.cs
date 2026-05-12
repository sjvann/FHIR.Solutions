using Fhir.VersionManager;

namespace Fhir.QueryBuilder.Metadata;

/// <summary>依 FHIR 版本解析 metadata 提供者。</summary>
public interface IMetadataResourceProviderResolver
{
    IMetadataResourceProvider GetProvider(FhirVersion version);
}

public sealed class MetadataResourceProviderResolver : IMetadataResourceProviderResolver
{
    private readonly IReadOnlyDictionary<FhirVersion, IMetadataResourceProvider> _providers;

    /// <summary>以單一實際提供者對應多個支援版本（連線後 metadata 依 <see cref="ICapabilityContext"/>）。</summary>
    public MetadataResourceProviderResolver(IMetadataResourceProvider sharedProvider, IEnumerable<FhirVersion> supportedVersions)
    {
        var keys = supportedVersions.Distinct().ToList();
        if (keys.Count == 0)
            keys = new List<FhirVersion> { FhirVersion.R5 };
        _providers = keys.ToDictionary(v => v, _ => sharedProvider);
    }

    public IMetadataResourceProvider GetProvider(FhirVersion version)
    {
        if (_providers.TryGetValue(version, out var provider))
            return provider;
        if (version == FhirVersion.Unknown && _providers.Count > 0)
            return _providers.Values.First();
        throw new NotSupportedException($"FHIR version {version} is not supported by metadata providers.");
    }
}
