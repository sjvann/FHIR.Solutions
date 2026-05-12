using Fhir.QueryBuilder.Common;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.VersionManager;
using Fhir.VersionManager.Capability;

namespace Fhir.QueryBuilder.Services.Interfaces
{
    /// <summary>
    /// 能力分析器介面
    /// </summary>
    public interface ICapabilityAnalyzer
    {
        Task<CapabilityAnalysisResult> AnalyzeCapabilityAsync(ICapabilityModel capability);

        Task<CapabilityComparisonResult> CompareCapabilitiesAsync(ICapabilityModel baseline, ICapabilityModel target);

        Task<bool> CheckFeatureSupportAsync(ICapabilityModel capability, string feature);

        Task<QueryStrategyRecommendation> GetQueryStrategyAsync(ICapabilityModel capability, string resourceType);
    }

    /// <summary>
    /// 能力分析結果
    /// </summary>
    public class CapabilityAnalysisResult
    {
        /// <summary>
        /// FHIR 版本
        /// </summary>
        public FhirVersion? FhirVersion { get; set; }

        /// <summary>
        /// 伺服器軟體資訊
        /// </summary>
        public string? ServerSoftware { get; set; }

        /// <summary>
        /// 支援的資源類型
        /// </summary>
        public List<string> SupportedResources { get; set; } = new();

        /// <summary>
        /// 支援的互動類型
        /// </summary>
        public List<string> SupportedInteractions { get; set; } = new();

        /// <summary>
        /// 支援的搜尋參數總數
        /// </summary>
        public int TotalSearchParameters { get; set; }

        /// <summary>
        /// 支援的格式
        /// </summary>
        public List<string> SupportedFormats { get; set; } = new();

        /// <summary>
        /// 安全性功能
        /// </summary>
        public SecurityFeatures Security { get; set; } = new();

        /// <summary>
        /// 進階功能支援
        /// </summary>
        public AdvancedFeatures Advanced { get; set; } = new();

        /// <summary>
        /// 相容性分數 (0-100)
        /// </summary>
        public int CompatibilityScore { get; set; }

        /// <summary>
        /// 分析建議
        /// </summary>
        public List<string> Recommendations { get; set; } = new();

        /// <summary>
        /// 潛在問題
        /// </summary>
        public List<string> PotentialIssues { get; set; } = new();
    }

    /// <summary>
    /// 能力比較結果
    /// </summary>
    public class CapabilityComparisonResult
    {
        /// <summary>
        /// 基準伺服器資訊
        /// </summary>
        public string BaselineServer { get; set; } = string.Empty;

        /// <summary>
        /// 目標伺服器資訊
        /// </summary>
        public string TargetServer { get; set; } = string.Empty;

        /// <summary>
        /// 相容性百分比
        /// </summary>
        public double CompatibilityPercentage { get; set; }

        /// <summary>
        /// 共同支援的資源
        /// </summary>
        public List<string> CommonResources { get; set; } = new();

        /// <summary>
        /// 基準獨有的資源
        /// </summary>
        public List<string> BaselineOnlyResources { get; set; } = new();

        /// <summary>
        /// 目標獨有的資源
        /// </summary>
        public List<string> TargetOnlyResources { get; set; } = new();

        /// <summary>
        /// 功能差異
        /// </summary>
        public List<FeatureDifference> FeatureDifferences { get; set; } = new();

        /// <summary>
        /// 遷移建議
        /// </summary>
        public List<string> MigrationRecommendations { get; set; } = new();
    }

    /// <summary>
    /// 查詢策略建議
    /// </summary>
    public class QueryStrategyRecommendation
    {
        /// <summary>
        /// 資源類型
        /// </summary>
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// 建議的搜尋參數
        /// </summary>
        public List<string> RecommendedSearchParameters { get; set; } = new();

        /// <summary>
        /// 支援的包含參數
        /// </summary>
        public List<string> SupportedIncludes { get; set; } = new();

        /// <summary>
        /// 建議的分頁大小
        /// </summary>
        public int RecommendedPageSize { get; set; } = 50;

        /// <summary>
        /// 是否支援排序
        /// </summary>
        public bool SupportsSorting { get; set; }

        /// <summary>
        /// 效能提示
        /// </summary>
        public List<string> PerformanceTips { get; set; } = new();

        /// <summary>
        /// 限制和注意事項
        /// </summary>
        public List<string> Limitations { get; set; } = new();
    }

    /// <summary>
    /// 安全性功能
    /// </summary>
    public class SecurityFeatures
    {
        /// <summary>
        /// 支援 OAuth 2.0
        /// </summary>
        public bool SupportsOAuth { get; set; }

        /// <summary>
        /// 支援 SMART on FHIR
        /// </summary>
        public bool SupportsSmart { get; set; }

        /// <summary>
        /// 支援 TLS
        /// </summary>
        public bool SupportsTls { get; set; }

        /// <summary>
        /// 支援的安全標籤
        /// </summary>
        public List<string> SecurityLabels { get; set; } = new();

        /// <summary>
        /// 授權端點
        /// </summary>
        public string? AuthorizeEndpoint { get; set; }

        /// <summary>
        /// 權杖端點
        /// </summary>
        public string? TokenEndpoint { get; set; }
    }

    /// <summary>
    /// 進階功能
    /// </summary>
    public class AdvancedFeatures
    {
        /// <summary>
        /// 支援 GraphQL
        /// </summary>
        public bool SupportsGraphQL { get; set; }

        /// <summary>
        /// 支援批次操作
        /// </summary>
        public bool SupportsBatch { get; set; }

        /// <summary>
        /// 支援交易
        /// </summary>
        public bool SupportsTransaction { get; set; }

        /// <summary>
        /// 支援訂閱
        /// </summary>
        public bool SupportsSubscription { get; set; }

        /// <summary>
        /// 支援大量資料匯出
        /// </summary>
        public bool SupportsBulkData { get; set; }

        /// <summary>
        /// 支援的操作
        /// </summary>
        public List<string> SupportedOperations { get; set; } = new();
    }

    /// <summary>
    /// 功能差異
    /// </summary>
    public class FeatureDifference
    {
        /// <summary>
        /// 功能名稱
        /// </summary>
        public string FeatureName { get; set; } = string.Empty;

        /// <summary>
        /// 基準狀態
        /// </summary>
        public bool BaselineSupported { get; set; }

        /// <summary>
        /// 目標狀態
        /// </summary>
        public bool TargetSupported { get; set; }

        /// <summary>
        /// 影響等級
        /// </summary>
        public ImpactLevel Impact { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// 影響等級
    /// </summary>
    public enum ImpactLevel
    {
        /// <summary>
        /// 低影響
        /// </summary>
        Low,

        /// <summary>
        /// 中等影響
        /// </summary>
        Medium,

        /// <summary>
        /// 高影響
        /// </summary>
        High,

        /// <summary>
        /// 關鍵影響
        /// </summary>
        Critical
    }
}
