using Fhir.QueryBuilder.Common;
using Fhir.VersionManager;
using Fhir.VersionManager.Capability;

namespace Fhir.QueryBuilder.Metadata;

/// <summary>由連線中的 <see cref="ICapabilityModel"/> 提供資源支援資訊（跨 R4/R4B/R5）。</summary>
public sealed class CapabilityMetadataProvider : IMetadataResourceProvider
{
    private readonly ICapabilityContext _context;

    public CapabilityMetadataProvider(ICapabilityContext context) => _context = context;

    public FhirVersion SupportedVersion => _context.CapabilityModel?.Version ?? FhirVersion.R5;

    public bool IsSupported(string resourceType)
    {
        if (string.IsNullOrWhiteSpace(resourceType))
            return false;

        if (_context.CapabilityModel == null)
            return true;

        return GetResourceMetadata(resourceType) != null;
    }

    public ResourceTypeMetadata? GetResourceMetadata(string resourceType)
    {
        var model = _context.CapabilityModel;
        if (model == null)
        {
            return new ResourceTypeMetadata
            {
                ResourceType = resourceType,
                SearchParameters = new List<SearchParameterMetadataEntry>()
            };
        }

        var rc = model.ServerResources.FirstOrDefault(r =>
            string.Equals(r.Type, resourceType, StringComparison.Ordinal));
        if (rc == null)
            return null;

        var list = new List<SearchParameterMetadataEntry>();
        foreach (var sp in rc.SearchParams)
        {
            var name = sp.Name ?? string.Empty;
            if (string.IsNullOrEmpty(name))
                continue;

            list.Add(new SearchParameterMetadataEntry
            {
                Name = name,
                Type = MapSearchParamType(sp.Type),
                SupportedModifiers = new List<string>()
            });
        }

        return new ResourceTypeMetadata
        {
            ResourceType = resourceType,
            SearchParameters = list
        };
    }

    private static SearchParameterType MapSearchParamType(string? t)
    {
        return t?.ToLowerInvariant() switch
        {
            "number" => SearchParameterType.Number,
            "date" => SearchParameterType.Date,
            "string" => SearchParameterType.String,
            "token" => SearchParameterType.Token,
            "reference" => SearchParameterType.Reference,
            "composite" => SearchParameterType.Composite,
            "quantity" => SearchParameterType.Quantity,
            "uri" => SearchParameterType.Uri,
            "special" => SearchParameterType.Special,
            _ => SearchParameterType.String
        };
    }
}
