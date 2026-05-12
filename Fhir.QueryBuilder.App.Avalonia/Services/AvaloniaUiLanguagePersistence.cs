using Fhir.QueryBuilder.Localization;

namespace Fhir.QueryBuilder.App.Avalonia.Services;

/// <summary>將 UI 語系寫入本機檔案（覆寫 appsettings 之 <c>DefaultUiLanguage</c>）。</summary>
internal static class AvaloniaUiLanguagePersistence
{
    private static string FilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FhirQueryBuilder", "ui-lang.txt");

    public static void TryLoad(QueryBuilderUiLanguageService lang)
    {
        try
        {
            if (!File.Exists(FilePath))
                return;
            var s = File.ReadAllText(FilePath).Trim();
            if (!string.IsNullOrWhiteSpace(s))
                lang.SetCulture(s);
        }
        catch
        {
            // 本機檔案不可用時略過
        }
    }

    public static void Save(QueryBuilderUiLanguageService lang)
    {
        try
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(FilePath, lang.CultureCode);
        }
        catch
        {
            /* ignore */
        }
    }
}
