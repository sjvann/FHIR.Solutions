namespace Fhir.QueryBuilder.Localization;

/// <summary>UI 語系代碼（與 localStorage／下拉選單 value 一致）。</summary>
public static class QueryBuilderUiCultureCodes
{
    public const string ZhTw = "zh-TW";
    public const string ZhCn = "zh-CN";
    public const string En = "en";
    public const string Ja = "ja";
    public const string Ko = "ko";

    /// <summary>語言選單固定順序（與 Avalonia ComboBox 等對齊）。</summary>
    public static readonly string[] PreferenceCodes = [ZhTw, ZhCn, En, Ja, Ko];

    public static string Normalize(string? code) => (code ?? "").Trim() switch
    {
        "zh-CN" or "zh-Hans" => ZhCn,
        "zh-TW" or "zh-Hant" => ZhTw,
        "en" or "en-US" or "en-GB" => En,
        "ja" or "ja-JP" => Ja,
        "ko" or "ko-KR" => Ko,
        _ => ZhTw,
    };
}
