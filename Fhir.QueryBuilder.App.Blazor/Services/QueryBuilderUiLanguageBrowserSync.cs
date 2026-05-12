using Fhir.QueryBuilder.Localization;
using Microsoft.JSInterop;

namespace Fhir.QueryBuilder.App.Blazor.Services;

/// <summary>將 UI 語系寫入／讀出瀏覽器 localStorage（覆寫 appsettings 的 DefaultUiLanguage）。</summary>
public static class QueryBuilderUiLanguageBrowserSync
{
    public static async Task TryLoadFromBrowserAsync(this QueryBuilderUiLanguageService lang, IJSRuntime js)
    {
        try
        {
            var stored = await js.InvokeAsync<string?>("fhirQueryBuilderInterop.getUiLang").ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(stored))
                lang.SetCulture(stored);
        }
        catch
        {
            // 首次載入或尚未注入 interop 時略過
        }
    }

    public static ValueTask PersistToBrowserAsync(this QueryBuilderUiLanguageService lang, IJSRuntime js) =>
        js.InvokeVoidAsync("fhirQueryBuilderInterop.setUiLang", lang.CultureCode);
}
