using Fhir.QueryBuilder.Common;
using Fhir.Resources.R5;
namespace Fhir.QueryBuilder.Metadata;

/// <summary>由連線中的 R5 <see cref="CapabilityStatement"/> 提供資源支援資訊。</summary>
public sealed class R5CapabilityMetadataProvider : IMetadataResourceProvider
{
    private readonly ICapabilityContext _context;

    public R5CapabilityMetadataProvider(ICapabilityContext context) => _context = context;

    public FhirVersion SupportedVersion => FhirVersion.R5;

    public bool IsSupported(string resourceType)
    {
        if (string.IsNullOrWhiteSpace(resourceType))
            return false;

        // 尚未取得 CapabilityStatement 前（離線組裝查詢），允許任意資源型別字串。
        if (_context.Capability == null)
            return true;

        return GetResourceMetadata(resourceType) != null;
    }

    public ResourceTypeMetadata? GetResourceMetadata(string resourceType)
    {
        var cap = _context.Capability;
        if (cap == null)
        {
            return new ResourceTypeMetadata
            {
                ResourceType = resourceType,
                SearchParameters = new List<SearchParameterMetadataEntry>()
            };
        }

        var rest = SelectServerRest(cap);
        if (rest?.Resource == null)
            return null;

        var rc = rest.Resource.FirstOrDefault(r =>
            string.Equals(CodeString(r.Type), resourceType, StringComparison.Ordinal));
        if (rc == null)
            return null;

        var list = new List<SearchParameterMetadataEntry>();
        foreach (var sp in rc.SearchParam ?? Enumerable.Empty<CapabilityStatement.RestComponent.RestResourceComponent.RestResourceSearchParamComponent>())
        {
            var name = (string?)sp.Name ?? string.Empty;
            if (string.IsNullOrEmpty(name))
                continue;

            list.Add(new SearchParameterMetadataEntry
            {
                Name = name,
                Type = MapSearchParamType(CodeString(sp.Type)),
                SupportedModifiers = new List<string>()
            });
        }

        return new ResourceTypeMetadata
        {
            ResourceType = resourceType,
            SearchParameters = list
        };
    }

    private static CapabilityStatement.RestComponent? SelectServerRest(CapabilityStatement? cap)
    {
        if (cap?.Rest == null || cap.Rest.Count == 0)
            return null;
        return cap.Rest.FirstOrDefault(r => string.Equals(CodeString(r.Mode), "server", StringComparison.OrdinalIgnoreCase))
               ?? cap.Rest[0];
    }

    private static string? CodeString(Fhir.TypeFramework.DataTypes.FhirCode? code) => code;

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
