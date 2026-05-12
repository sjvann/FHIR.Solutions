namespace Fhir.Terminology.Core;

/// <summary>
/// 將組態、套件 manifest、查詢字串等來源正規化為慣用之 FHIR 規格版本字串（與資源本身的 business <c>version</c> 無關）。
/// </summary>
public static class FhirSpecVersionNormalizer
{
    /// <summary>
    /// 若 <paramref name="input"/> 為空白則回傳 <paramref name="fallbackWhenEmpty"/>。
    /// </summary>
    public static string Normalize(string? input, string fallbackWhenEmpty = "5.0.0")
    {
        if (string.IsNullOrWhiteSpace(input))
            return fallbackWhenEmpty;

        var s = input.Trim();
        if (string.Equals(s, "R4", StringComparison.OrdinalIgnoreCase))
            return "4.0.1";
        if (string.Equals(s, "R5", StringComparison.OrdinalIgnoreCase))
            return "5.0.0";
        if (string.Equals(s, "R6", StringComparison.OrdinalIgnoreCase))
            return "6.0.0";

        return s;
    }
}
