namespace Fhir.QueryBuilder.Common;

/// <summary>FHIR 協定版本（Query Builder 用途）。</summary>
public enum FhirVersion
{
    Unknown = 0,
    R4 = 4,
    R4B = 40,
    R5 = 5,
}

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
}
