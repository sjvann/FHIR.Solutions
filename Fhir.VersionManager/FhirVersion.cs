namespace Fhir.VersionManager;

/// <summary>FHIR 協定版本（跨應用一致列舉）。</summary>
public enum FhirVersion
{
    Unknown = 0,
    R4 = 4,
    R4B = 40,
    R5 = 5,
}

/// <summary>當 URL、Capability 內 <c>fhirVersion</c>、與使用者宣告不一致時的合併策略。</summary>
public enum FhirVersionResolutionStrategy
{
    /// <summary>優先使用 <c>/metadata</c> 之 <c>fhirVersion</c>；缺省時再用 URL 與宣告。</summary>
    PreferDetected = 0,

    /// <summary>優先使用設定／UI 宣告；缺省時仍回落為偵測結果（反序列化仍以偵測為準）。</summary>
    PreferDeclared = 1,
}
