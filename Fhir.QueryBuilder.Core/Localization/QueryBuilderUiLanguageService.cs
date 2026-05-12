using Fhir.QueryBuilder.Configuration;
using Microsoft.Extensions.Options;

namespace Fhir.QueryBuilder.Localization;

/// <summary>
/// Query Builder UI 語系（不依賴 ICU；字串由 <see cref="QueryBuilderUiStrings"/> 提供）。
/// Blazor／Avalonia／WinForms 共用；瀏覽器／檔案持久化由各主機程式處理。
/// </summary>
public sealed class QueryBuilderUiLanguageService
{
    public const string DefaultCultureCode = QueryBuilderUiCultureCodes.ZhTw;

    public event Action? Changed;

    public string CultureCode { get; private set; }

    public QueryBuilderUiStrings Strings { get; private set; }

    public QueryBuilderUiLanguageService(IOptions<QueryBuilderAppSettings> appSettings)
    {
        var configured = appSettings.Value.DefaultUiLanguage;
        var initial = string.IsNullOrWhiteSpace(configured)
            ? DefaultCultureCode
            : NormalizeCultureCode(configured.Trim());
        CultureCode = initial;
        Strings = QueryBuilderUiStrings.For(initial);
    }

    public static string NormalizeCultureCode(string? code) => QueryBuilderUiCultureCodes.Normalize(code);

    public void SetCulture(string? code)
    {
        var n = NormalizeCultureCode(code);
        if (string.Equals(CultureCode, n, StringComparison.Ordinal))
            return;

        CultureCode = n;
        Strings = QueryBuilderUiStrings.For(n);
        Changed?.Invoke();
    }
}
