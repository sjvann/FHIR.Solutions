namespace Fhir.VersionManager.Capability;

/// <summary>解析 <c>/metadata</c> 後之版本與統一模型。</summary>
public sealed class CapabilityParseResult
{
    /// <summary>自 JSON <c>fhirVersion</c> 等推得之伺服器線別。</summary>
    public required FhirVersion DetectedVersion { get; init; }

    /// <summary>應用程式宣告／選取之線別（顯示與查詢偏好）。</summary>
    public required FhirVersion SelectedVersion { get; init; }

    /// <summary>當偵測與宣告皆已知且不一致時之警告（可為 null）。</summary>
    public string? MismatchWarning { get; init; }

    public required ICapabilityModel Model { get; init; }

    public required string Json { get; init; }
}
