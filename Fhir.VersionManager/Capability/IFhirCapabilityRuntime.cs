namespace Fhir.VersionManager.Capability;

/// <summary>將 metadata JSON 依正確 FHIR 線別反序列化並產出 <see cref="ICapabilityModel"/>。</summary>
public interface IFhirCapabilityRuntime
{
    /// <param name="metadataJson"><c>/metadata</c> 回應本文。</param>
    /// <param name="baseUrl">連線基底 URL（供 URL 路徑線索）。</param>
    /// <param name="selectedVersion">應用宣告之線別；未知時傳 <see cref="FhirVersion.Unknown"/>。</param>
    /// <param name="strategy">合併策略。</param>
    CapabilityParseResult ParseMetadata(string metadataJson, string? baseUrl, FhirVersion selectedVersion,
        FhirVersionResolutionStrategy strategy);
}
