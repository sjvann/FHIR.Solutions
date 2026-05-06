using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using Fhir.TypeFramework.Performance;

namespace Fhir.TypeFramework.Validation;

public static class ValidationFramework
{
    // 基本驗證工具（可重用）
    public static bool ValidateStringLength(string? value, int maxLength)
        => value is null || value.Length <= maxLength;

    public static bool ValidateStringByteSize(string? value, int maxBytes)
        => value is null || Encoding.UTF8.GetByteCount(value) <= maxBytes;

    public static bool ValidateRegex(string? value, string pattern)
    {
        if (value is null) return true;
        var re = ValidationOptimizer.GetCachedRegex(pattern);
        return re.IsMatch(value);
    }

    public static bool ValidatePositiveInteger(int value) => value > 0;

    // FHIR 特定驗證規則（先提供高使用頻率的一組）
    // - id: [A-Za-z0-9\-\.]{1,64}
    private const string FhirIdPattern = "^[A-Za-z0-9\\-\\.]{1,64}$";

    public static bool ValidateFhirId(string? value)
        => value is null || ValidateRegex(value, FhirIdPattern);

    public static bool ValidateFhirUri(string? value)
    {
        if (value is null) return true;

        // 寬鬆處理：只要是絕對 URI 或相對/URN 形式即可；避免過度嚴格導致誤判。
        if (Uri.TryCreate(value, UriKind.Absolute, out _)) return true;
        if (value.StartsWith("urn:", StringComparison.OrdinalIgnoreCase)) return true;
        return Uri.TryCreate(value, UriKind.Relative, out _);
    }

    public static bool ValidateFhirCode(string? value)
    {
        if (value is null) return true;
        // FHIR code: no leading/trailing whitespace; no internal " \t\r\n"?
        // 這裡先採用最常見的「不允許控制字元」的安全子集合。
        return value.Length == value.Trim().Length && !value.Any(char.IsControl);
    }

    public static bool ValidateFhirCanonical(string? value) => ValidateFhirUri(value);
    public static bool ValidateFhirOid(string? value) => value is null || value.StartsWith("urn:oid:", StringComparison.OrdinalIgnoreCase) || ValidateRegex(value, "^[0-2](\\.(0|[1-9][0-9]*))+$");
    public static bool ValidateFhirUuid(string? value) => value is null || value.StartsWith("urn:uuid:", StringComparison.OrdinalIgnoreCase) || Guid.TryParse(value, out _);
    public static bool ValidateFhirBase64Binary(string? value)
    {
        if (value is null) return true;
        Span<byte> buffer = stackalloc byte[Math.Min(value.Length, 4096)];
        return Convert.TryFromBase64String(value, buffer, out _);
    }

    // 複雜驗證邏輯（先保留擴展點；目前專案內 Complex Types 尚未完整齊備）
    public static IEnumerable<ValidationResult> ValidateExtension(object? extension, ValidationContext context)
    {
        _ = extension;
        _ = context;
        yield break;
    }
}

