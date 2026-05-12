using Fhir.TypeFramework.Serialization;

namespace Fhir.VersionManager.Capability;

internal static class CapabilityModelFactory
{
    public static ICapabilityModel FromR4(string json)
    {
        var cap = FhirJsonSerializer.Deserialize<Fhir.Resources.R4.CapabilityStatement>(json)
                  ?? throw new InvalidOperationException("Failed to deserialize R4 CapabilityStatement.");
        return Map(FhirVersion.R4, (string?)cap.FhirVersion, cap);
    }

    public static ICapabilityModel FromR4B(string json)
    {
        var cap = FhirJsonSerializer.Deserialize<Fhir.Resources.R4B.CapabilityStatement>(json)
                  ?? throw new InvalidOperationException("Failed to deserialize R4B CapabilityStatement.");
        return Map(FhirVersion.R4B, (string?)cap.FhirVersion, cap);
    }

    public static ICapabilityModel FromR5(string json)
    {
        var cap = FhirJsonSerializer.Deserialize<Fhir.Resources.R5.CapabilityStatement>(json)
                  ?? throw new InvalidOperationException("Failed to deserialize R5 CapabilityStatement.");
        return Map(FhirVersion.R5, (string?)cap.FhirVersion, cap);
    }

    private static CapabilityModel Map(FhirVersion version, string? fhirVersionEl, Fhir.Resources.R4.CapabilityStatement cap)
    {
        var rest = SelectServerRestR4(cap);
        return new CapabilityModel
        {
            Version = version,
            FhirVersionElement = fhirVersionEl,
            SoftwareName = (string?)cap.Software?.Name,
            ImplementationGuideUrls = cap.ImplementationGuide?.Select(c => (string?)c).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
            Formats = cap.Format?.Select(f => (string?)f).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
            ServerResources = MapResourcesR4(rest?.Resource)
        };
    }

    private static CapabilityModel Map(FhirVersion version, string? fhirVersionEl, Fhir.Resources.R4B.CapabilityStatement cap)
    {
        var rest = SelectServerRestR4B(cap);
        return new CapabilityModel
        {
            Version = version,
            FhirVersionElement = fhirVersionEl,
            SoftwareName = (string?)cap.Software?.Name,
            ImplementationGuideUrls = cap.ImplementationGuide?.Select(c => (string?)c).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
            Formats = cap.Format?.Select(f => (string?)f).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
            ServerResources = MapResourcesR4B(rest?.Resource)
        };
    }

    private static CapabilityModel Map(FhirVersion version, string? fhirVersionEl, Fhir.Resources.R5.CapabilityStatement cap)
    {
        var rest = SelectServerRestR5(cap);
        return new CapabilityModel
        {
            Version = version,
            FhirVersionElement = fhirVersionEl,
            SoftwareName = (string?)cap.Software?.Name,
            ImplementationGuideUrls = cap.ImplementationGuide?.Select(c => (string?)c).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
            Formats = cap.Format?.Select(f => (string?)f).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
            ServerResources = MapResourcesR5(rest?.Resource)
        };
    }

    private static Fhir.Resources.R4.CapabilityStatement.RestComponent? SelectServerRestR4(Fhir.Resources.R4.CapabilityStatement? cap)
    {
        if (cap?.Rest == null || cap.Rest.Count == 0)
            return null;
        var server = cap.Rest.FirstOrDefault(r =>
            string.Equals((string?)r.Mode, "server", StringComparison.OrdinalIgnoreCase));
        if (server?.Resource is { Count: > 0 })
            return server;
        return cap.Rest.FirstOrDefault(r => r.Resource is { Count: > 0 }) ?? server ?? cap.Rest[0];
    }

    private static Fhir.Resources.R4B.CapabilityStatement.RestComponent? SelectServerRestR4B(Fhir.Resources.R4B.CapabilityStatement? cap)
    {
        if (cap?.Rest == null || cap.Rest.Count == 0)
            return null;
        var server = cap.Rest.FirstOrDefault(r =>
            string.Equals((string?)r.Mode, "server", StringComparison.OrdinalIgnoreCase));
        if (server?.Resource is { Count: > 0 })
            return server;
        return cap.Rest.FirstOrDefault(r => r.Resource is { Count: > 0 }) ?? server ?? cap.Rest[0];
    }

    private static Fhir.Resources.R5.CapabilityStatement.RestComponent? SelectServerRestR5(Fhir.Resources.R5.CapabilityStatement? cap)
    {
        if (cap?.Rest == null || cap.Rest.Count == 0)
            return null;
        var server = cap.Rest.FirstOrDefault(r =>
            string.Equals((string?)r.Mode, "server", StringComparison.OrdinalIgnoreCase));
        if (server?.Resource is { Count: > 0 })
            return server;
        return cap.Rest.FirstOrDefault(r => r.Resource is { Count: > 0 }) ?? server ?? cap.Rest[0];
    }

    private static IReadOnlyList<CapabilityRestResourceModel> MapResourcesR4(
        List<Fhir.Resources.R4.CapabilityStatement.RestComponent.RestResourceComponent>? resources)
    {
        if (resources == null)
            return Array.Empty<CapabilityRestResourceModel>();
        var list = new List<CapabilityRestResourceModel>();
        foreach (var rc in resources)
        {
            list.Add(new CapabilityRestResourceModel
            {
                Type = (string?)rc.Type,
                InteractionCodes = rc.Interaction?.Select(i => (string?)i.Code).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
                SearchParams = rc.SearchParam?.Select(sp => new CapabilitySearchParamModel
                {
                    Name = (string?)sp.Name,
                    Type = (string?)sp.Type,
                    Documentation = (string?)sp.Documentation
                }).ToList() ?? new List<CapabilitySearchParamModel>(),
                SearchIncludes = rc.SearchInclude?.Select(s => (string?)s).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
                SearchRevIncludes = rc.SearchRevInclude?.Select(s => (string?)s).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>()
            });
        }

        return list;
    }

    private static IReadOnlyList<CapabilityRestResourceModel> MapResourcesR4B(
        List<Fhir.Resources.R4B.CapabilityStatement.RestComponent.RestResourceComponent>? resources)
    {
        if (resources == null)
            return Array.Empty<CapabilityRestResourceModel>();
        var list = new List<CapabilityRestResourceModel>();
        foreach (var rc in resources)
        {
            list.Add(new CapabilityRestResourceModel
            {
                Type = (string?)rc.Type,
                InteractionCodes = rc.Interaction?.Select(i => (string?)i.Code).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
                SearchParams = rc.SearchParam?.Select(sp => new CapabilitySearchParamModel
                {
                    Name = (string?)sp.Name,
                    Type = (string?)sp.Type,
                    Documentation = (string?)sp.Documentation
                }).ToList() ?? new List<CapabilitySearchParamModel>(),
                SearchIncludes = rc.SearchInclude?.Select(s => (string?)s).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
                SearchRevIncludes = rc.SearchRevInclude?.Select(s => (string?)s).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>()
            });
        }

        return list;
    }

    private static IReadOnlyList<CapabilityRestResourceModel> MapResourcesR5(
        List<Fhir.Resources.R5.CapabilityStatement.RestComponent.RestResourceComponent>? resources)
    {
        if (resources == null)
            return Array.Empty<CapabilityRestResourceModel>();
        var list = new List<CapabilityRestResourceModel>();
        foreach (var rc in resources)
        {
            list.Add(new CapabilityRestResourceModel
            {
                Type = (string?)rc.Type,
                InteractionCodes = rc.Interaction?.Select(i => (string?)i.Code).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
                SearchParams = rc.SearchParam?.Select(sp => new CapabilitySearchParamModel
                {
                    Name = (string?)sp.Name,
                    Type = (string?)sp.Type,
                    Documentation = (string?)sp.Documentation
                }).ToList() ?? new List<CapabilitySearchParamModel>(),
                SearchIncludes = rc.SearchInclude?.Select(s => (string?)s).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>(),
                SearchRevIncludes = rc.SearchRevInclude?.Select(s => (string?)s).Where(s => !string.IsNullOrEmpty(s)).Cast<string>().ToList() ?? new List<string>()
            });
        }

        return list;
    }
}
