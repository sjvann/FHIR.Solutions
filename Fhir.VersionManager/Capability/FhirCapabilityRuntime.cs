namespace Fhir.VersionManager.Capability;

public sealed class FhirCapabilityRuntime : IFhirCapabilityRuntime
{
    public CapabilityParseResult ParseMetadata(string metadataJson, string? baseUrl, FhirVersion selectedVersion,
        FhirVersionResolutionStrategy strategy)
    {
        if (string.IsNullOrWhiteSpace(metadataJson))
            throw new ArgumentException("Metadata JSON is required.", nameof(metadataJson));

        var fromJson = FhirVersionDetector.TryParseFromMetadataJson(metadataJson);
        var fromUrl = FhirVersionDetector.FromBaseUrl(baseUrl);
        var declared = selectedVersion;
        var deserializeVersion = FhirVersionDetector.ResolveDeserializeVersion(fromJson, fromUrl, declared);
        var resolvedSelected = FhirVersionDetector.ResolveSelectedVersion(fromJson, fromUrl, declared, strategy);
        var detectedDisplay = fromJson != FhirVersion.Unknown
            ? fromJson
            : fromUrl != FhirVersion.Unknown
                ? fromUrl
                : deserializeVersion;

        ICapabilityModel model = deserializeVersion switch
        {
            FhirVersion.R4 => CapabilityModelFactory.FromR4(metadataJson),
            FhirVersion.R4B => CapabilityModelFactory.FromR4B(metadataJson),
            _ => CapabilityModelFactory.FromR5(metadataJson)
        };

        var warn = FhirVersionDetector.TryBuildMismatchWarning(deserializeVersion, resolvedSelected);

        return new CapabilityParseResult
        {
            DetectedVersion = detectedDisplay,
            SelectedVersion = resolvedSelected,
            MismatchWarning = warn,
            Model = model,
            Json = metadataJson
        };
    }
}
