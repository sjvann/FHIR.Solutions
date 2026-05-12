using System.ComponentModel.DataAnnotations;
using Fhir.Auth.TokenServer.Configuration;
using Fhir.VersionManager;

namespace Fhir.QueryBuilder.Configuration
{
    public class QueryBuilderAppSettings
    {
        public const string SectionName = "Fhir.QueryBuilder";

        /// <summary>
        /// Blazor 等 UI 預設語系（例：<c>zh-TW</c>、<c>zh-CN</c>、<c>en</c>、<c>ja</c>、<c>ko</c>）。
        /// 空白則由主機程式決定（Blazor 預設為繁體中文）；使用者若曾於瀏覽器選語系，仍以 localStorage 為準。
        /// </summary>
        public string DefaultUiLanguage { get; set; } = "";

        /// <summary>預設 FHIR 線別簡寫：R4、R4B、R5（與 <see cref="FhirVersionParser.ParseFromShortName"/> 一致）。</summary>
        public string DefaultFhirVersion { get; set; } = "R5";

        /// <summary>URL／metadata 與宣告版本合併策略。</summary>
        public FhirVersionResolutionStrategy FhirVersionResolution { get; set; } = FhirVersionResolutionStrategy.PreferDetected;

        [Required]
        [Url]
        public string DefaultServerUrl { get; set; } = "https://server.fire.ly";

        [Range(5, 300)]
        public int RequestTimeoutSeconds { get; set; } = 30;

        public bool EnableLogging { get; set; } = true;

        public bool EnableCaching { get; set; } = true;

        [Range(1, 1000)]
        public int CacheExpirationMinutes { get; set; } = 30;

        public Dictionary<string, string> CustomHeaders { get; set; } = new();

        public List<string> RecentServers { get; set; } = new();

        [Range(1, 50)]
        public int MaxRecentServers { get; set; } = 10;

        public bool AutoSaveQueries { get; set; } = true;

        public string QueryHistoryPath { get; set; } = "QueryHistory";

        [Range(1, 10000)]
        public int MaxQueryHistoryItems { get; set; } = 100;

        // UI Settings
        public UiSettings Ui { get; set; } = new();

        // Performance Settings
        public PerformanceSettings Performance { get; set; } = new();

        // Security Settings
        public SecuritySettings Security { get; set; } = new();

        /// <summary>OAuth／SMART：設定來自獨立組件 <c>Fhir.Auth.TokenServer</c>（<see cref="TokenServerOptions"/>）。</summary>
        public TokenServerOptions Smart { get; set; } = new();

        // Export Settings
        public ExportSettings Export { get; set; } = new();
    }

    public class UiSettings
    {
        public string Theme { get; set; } = "Light";
        public string Language { get; set; } = "en-US";
        public bool ShowAdvancedOptions { get; set; } = false;
        public bool AutoExpandTreeView { get; set; } = true;
        public int DefaultPageSize { get; set; } = 20;
        public bool ShowLineNumbers { get; set; } = true;
        public bool WordWrap { get; set; } = true;
        public string FontFamily { get; set; } = "Consolas";
        public int FontSize { get; set; } = 12;
    }

    public class PerformanceSettings
    {
        [Range(1, 100)]
        public int MaxConcurrentRequests { get; set; } = 5;

        [Range(1, 10000)]
        public int MaxResultsPerPage { get; set; } = 1000;

        public bool EnableResultStreaming { get; set; } = false;

        [Range(1, 3600)]
        public int ConnectionPoolTimeoutSeconds { get; set; } = 300;

        public bool EnableCompression { get; set; } = true;

        [Range(1, 100)]
        public int RetryAttempts { get; set; } = 3;

        [Range(1, 60)]
        public int RetryDelaySeconds { get; set; } = 5;
    }

    public class SecuritySettings
    {
        public bool ValidateSslCertificates { get; set; } = true;
        public bool AllowSelfSignedCertificates { get; set; } = false;
        public List<string> TrustedCertificateThumbprints { get; set; } = new();
        public bool EnableTokenEncryption { get; set; } = true;
        public string TokenEncryptionKey { get; set; } = string.Empty;
        public bool LogSensitiveData { get; set; } = false;
        public List<string> AllowedHosts { get; set; } = new();
        public bool RequireHttps { get; set; } = true;
    }

    public class ExportSettings
    {
        public string DefaultExportPath { get; set; } = "Exports";
        public string DefaultExportFormat { get; set; } = "JSON";
        public List<string> SupportedFormats { get; set; } = new() { "JSON", "XML", "CSV" };
        public bool IncludeMetadata { get; set; } = true;
        public bool PrettyPrint { get; set; } = true;
        public string DateTimeFormat { get; set; } = "yyyy-MM-ddTHH:mm:ss.fffZ";
        public bool CompressExports { get; set; } = false;
        public int MaxExportSizeMB { get; set; } = 100;
    }
}
