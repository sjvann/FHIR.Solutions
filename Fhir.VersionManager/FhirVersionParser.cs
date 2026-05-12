namespace Fhir.VersionManager;

/// <summary>由 CapabilityStatement.fhirVersion 等字串推斷 <see cref="FhirVersion"/>。</summary>
public static class FhirVersionParser
{
    public static FhirVersion ParseFromCapabilityString(string? versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
            return FhirVersion.Unknown;

        var v = versionString.Trim();
        if (v.StartsWith("4.3", StringComparison.Ordinal) || v.Contains("R4B", StringComparison.OrdinalIgnoreCase))
            return FhirVersion.R4B;
        if (v.StartsWith("4.", StringComparison.Ordinal))
            return FhirVersion.R4;
        if (v.StartsWith("5.", StringComparison.Ordinal))
            return FhirVersion.R5;

        return FhirVersion.Unknown;
    }

    /// <summary>將 UI／設定檔之簡寫（R4、R4B、R5）轉為列舉。</summary>
    public static FhirVersion ParseFromShortName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return FhirVersion.Unknown;
        return name.Trim().ToUpperInvariant() switch
        {
            "R4" => FhirVersion.R4,
            "R4B" => FhirVersion.R4B,
            "R5" => FhirVersion.R5,
            _ => FhirVersion.Unknown
        };
    }

    public static string ToShortName(FhirVersion version) =>
        version switch
        {
            FhirVersion.R4 => "R4",
            FhirVersion.R4B => "R4B",
            FhirVersion.R5 => "R5",
            _ => "Unknown"
        };
}
