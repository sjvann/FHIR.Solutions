using Fhir.VersionManager;
using Fhir.VersionManager.Capability;

namespace Fhir.QueryBuilder.Metadata;

/// <summary>目前連線伺服器之 Capability（由 <see cref="Services.FhirQueryService"/> 更新）。</summary>
public interface ICapabilityContext
{
    string? BaseUrl { get; }

    /// <summary>統一模型（依伺服器 JSON 正確線別反序列化後映射）。</summary>
    ICapabilityModel? CapabilityModel { get; }

    /// <summary>最近一次 <c>/metadata</c> 回應 JSON（用於列舉資源型別之後援解析）。</summary>
    string? LastCapabilityJson { get; }

    /// <summary>自 metadata 推得之伺服器線別。</summary>
    FhirVersion DetectedFhirVersion { get; }

    /// <summary>應用程式選擇／宣告之作用線別。</summary>
    FhirVersion SelectedFhirVersion { get; }

    string? VersionMismatchWarning { get; }

    /// <summary>最近一次連線之完整解析結果（供相容性／分析重用）。</summary>
    CapabilityParseResult? LastParseResult { get; }

    void SetConnection(string baseUrl, CapabilityParseResult parseResult);

    /// <summary>清除連線狀態（例如使用者切換 FHIR 宣告版本）。</summary>
    void Clear();
}

public sealed class CapabilityContext : ICapabilityContext
{
    public string? BaseUrl { get; private set; }

    public ICapabilityModel? CapabilityModel { get; private set; }

    public string? LastCapabilityJson { get; private set; }

    public FhirVersion DetectedFhirVersion { get; private set; }

    public FhirVersion SelectedFhirVersion { get; private set; }

    public string? VersionMismatchWarning { get; private set; }

    public CapabilityParseResult? LastParseResult { get; private set; }

    public void SetConnection(string baseUrl, CapabilityParseResult parseResult)
    {
        BaseUrl = baseUrl.TrimEnd('/');
        CapabilityModel = parseResult.Model;
        LastCapabilityJson = parseResult.Json;
        DetectedFhirVersion = parseResult.DetectedVersion;
        SelectedFhirVersion = parseResult.SelectedVersion;
        VersionMismatchWarning = parseResult.MismatchWarning;
        LastParseResult = parseResult;
    }

    public void Clear()
    {
        BaseUrl = null;
        CapabilityModel = null;
        LastCapabilityJson = null;
        DetectedFhirVersion = FhirVersion.Unknown;
        SelectedFhirVersion = FhirVersion.Unknown;
        VersionMismatchWarning = null;
        LastParseResult = null;
    }
}
