using Fhir.QueryBuilder.Common;

namespace Fhir.QueryBuilder.Metadata;

/// <summary>依 FHIR 版本解析 metadata 提供者。</summary>
public interface IMetadataResourceProviderResolver
{
    IMetadataResourceProvider GetProvider(FhirVersion version);
}

public sealed class MetadataResourceProviderResolver : IMetadataResourceProviderResolver
{
    private readonly Dictionary<FhirVersion, IMetadataResourceProvider> _providers;

    public MetadataResourceProviderResolver(IEnumerable<IMetadataResourceProvider> providers)
    {
        _providers = providers.ToDictionary(p => p.SupportedVersion, p => p);
    }

    public IMetadataResourceProvider GetProvider(FhirVersion version)
    {
        return _providers.TryGetValue(version, out var provider)
            ? provider
            : throw new NotSupportedException($"FHIR version {version} is not supported by metadata providers.");
    }
}
