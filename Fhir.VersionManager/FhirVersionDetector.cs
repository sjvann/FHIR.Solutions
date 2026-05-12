using System.Text.Json;

namespace Fhir.VersionManager;

/// <summary>自 Base URL 與 metadata JSON 推斷 FHIR 線別。</summary>
public static class FhirVersionDetector
{
    /// <summary>自 JSON 根屬性 <c>fhirVersion</c> 推斷（失敗則 <see cref="FhirVersion.Unknown"/>）。</summary>
    public static FhirVersion TryParseFromMetadataJson(string? metadataJson)
    {
        if (string.IsNullOrWhiteSpace(metadataJson))
            return FhirVersion.Unknown;
        try
        {
            using var doc = JsonDocument.Parse(metadataJson);
            if (doc.RootElement.TryGetProperty("fhirVersion", out var el))
            {
                var s = el.ValueKind == JsonValueKind.String ? el.GetString() : el.ToString();
                return FhirVersionParser.ParseFromCapabilityString(s);
            }
        }
        catch (JsonException)
        {
            return FhirVersion.Unknown;
        }

        return FhirVersion.Unknown;
    }

    /// <summary>常見 URL 路徑線索（不涵蓋所有伺服器慣例）。</summary>
    public static FhirVersion FromBaseUrl(string? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            return FhirVersion.Unknown;
        var u = baseUrl.TrimEnd('/');
        if (u.Contains("r4b", StringComparison.OrdinalIgnoreCase) || u.Contains("/4.3", StringComparison.OrdinalIgnoreCase) || u.Contains("baseR4B", StringComparison.OrdinalIgnoreCase))
            return FhirVersion.R4B;
        if (u.Contains("baseR4", StringComparison.OrdinalIgnoreCase) ||
            u.Contains("/r4", StringComparison.OrdinalIgnoreCase) || u.Contains("/R4/", StringComparison.Ordinal) ||
            u.Contains("fhir/r4", StringComparison.OrdinalIgnoreCase))
            return FhirVersion.R4;
        if (u.Contains("baseR5", StringComparison.OrdinalIgnoreCase) ||
            u.Contains("/r5", StringComparison.OrdinalIgnoreCase) || u.Contains("/R5/", StringComparison.Ordinal) ||
            u.Contains("fhir/r5", StringComparison.OrdinalIgnoreCase))
            return FhirVersion.R5;
        return FhirVersion.Unknown;
    }

    /// <summary>決定 POCO 反序列化線別：一律以 metadata 之 <c>fhirVersion</c> 為優先，避免宣告與實際 JSON 結構不符。</summary>
    public static FhirVersion ResolveDeserializeVersion(FhirVersion fromJson, FhirVersion fromUrl, FhirVersion declared)
    {
        if (fromJson != FhirVersion.Unknown)
            return fromJson;
        if (fromUrl != FhirVersion.Unknown)
            return fromUrl;
        if (declared != FhirVersion.Unknown)
            return declared;
        return FhirVersion.R5;
    }

    /// <summary>應用程式顯示／查詢偏好線別。</summary>
    public static FhirVersion ResolveSelectedVersion(FhirVersion fromJson, FhirVersion fromUrl, FhirVersion declared,
        FhirVersionResolutionStrategy strategy)
    {
        if (strategy == FhirVersionResolutionStrategy.PreferDeclared && declared != FhirVersion.Unknown)
            return declared;
        if (fromJson != FhirVersion.Unknown)
            return fromJson;
        if (fromUrl != FhirVersion.Unknown)
            return fromUrl;
        if (declared != FhirVersion.Unknown)
            return declared;
        return FhirVersion.R5;
    }

    public static string? TryBuildMismatchWarning(FhirVersion deserializeVersion, FhirVersion selectedVersion)
    {
        if (selectedVersion == FhirVersion.Unknown || deserializeVersion == FhirVersion.Unknown)
            return null;
        if (selectedVersion == deserializeVersion)
            return null;
        return
            $"Declared FHIR version ({FhirVersionParser.ToShortName(selectedVersion)}) differs from parsed server metadata ({FhirVersionParser.ToShortName(deserializeVersion)}). JSON is deserialized using the parsed line.";
    }
}
