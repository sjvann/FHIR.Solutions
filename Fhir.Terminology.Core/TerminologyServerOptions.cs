using System.ComponentModel.DataAnnotations;

namespace Fhir.Terminology.Core;

/// <summary>術語 App 之 SQLite 與主機相關組態（搭配 <see cref="Microsoft.Extensions.Options"/>）。</summary>
public sealed class TerminologyServerOptions
{
    public const string SectionName = "Terminology";

    /// <summary>SQLite 連線字串（通常來自 <c>ConnectionStrings:Terminology</c>）。</summary>
    [Required]
    public string ConnectionString { get; set; } = "Data Source=terminology.db";

    /// <summary>
    /// 對外公開之 FHIR 服務根 URI（無尾階斜線），用於 <c>CapabilityStatement.url</c>、<c>TerminologyCapabilities.url</c> 等。
    /// </summary>
    [Required]
    public string MetadataPublicBaseUri { get; set; } = "http://localhost/fhir";

    /// <summary>於 CapabilityStatement 宣告之 FHIR 版本字串（例如 <c>5.0.0</c>）。</summary>
    public string FhirVersionLabel { get; set; } = "5.0.0";

    /// <summary>
    /// 管理匯入／種子資源時若未於請求指定規格版本，預設套用之本機 FHIR 規格版本（例 <c>5.0.0</c>）。
    /// 空白時視為使用 <see cref="FhirVersionLabel"/>。
    /// </summary>
    public string DefaultImportFhirSpecVersion { get; set; } = "";

    /// <summary>
    /// 組合 <see cref="MetadataPublicBaseUri"/> 與路徑後之 CapabilityStatement 識別 URI（慣例 <c…/metadata</c>）。
    /// </summary>
    public string GetCapabilityStatementUrl() =>
        $"{MetadataPublicBaseUri.TrimEnd('/')}/metadata";

    /// <summary>TerminologyCapabilities 資源的公開 <c>url</c>。</summary>
    public string GetTerminologyCapabilitiesUrl() =>
        $"{MetadataPublicBaseUri.TrimEnd('/')}/terminology-capabilities";

    /// <summary>解析後之預設匯入 FHIR 規格版本（供種入／批次貼上）。</summary>
    public string GetEffectiveDefaultImportFhirSpecVersion() =>
        string.IsNullOrWhiteSpace(DefaultImportFhirSpecVersion)
            ? FhirSpecVersionNormalizer.Normalize(FhirVersionLabel)
            : FhirSpecVersionNormalizer.Normalize(DefaultImportFhirSpecVersion);
}
