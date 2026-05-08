using Fhir.QueryBuilder.Common;
using Fhir.QueryBuilder.QueryCore.Validation;

namespace Fhir.QueryBuilder.QueryCore
{
    /// <summary>
    /// 版本感知查詢建構器介面
    /// </summary>
    public interface IVersionAwareQueryBuilder
    {
        /// <summary>
        /// 指定 FHIR 版本
        /// </summary>
        /// <param name="version">FHIR 版本</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder ForVersion(FhirVersion version);

        /// <summary>
        /// 指定資源類型
        /// </summary>
        /// <param name="resourceType">資源類型</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder ForResource(string resourceType);

        /// <summary>
        /// 指定伺服器 URL
        /// </summary>
        /// <param name="serverUrl">伺服器 URL</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder ForServer(string serverUrl);

        /// <summary>
        /// 新增搜尋參數
        /// </summary>
        /// <param name="name">參數名稱</param>
        /// <param name="value">參數值</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder AddParameter(string name, string value);

        /// <summary>
        /// 新增搜尋參數（帶修飾符）
        /// </summary>
        /// <param name="name">參數名稱</param>
        /// <param name="modifier">修飾符</param>
        /// <param name="value">參數值</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder AddParameter(string name, string modifier, string value);

        /// <summary>
        /// 設定分頁
        /// </summary>
        /// <param name="count">每頁數量</param>
        /// <param name="offset">偏移量</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder WithPaging(int count, int offset = 0);

        /// <summary>
        /// 設定排序
        /// </summary>
        /// <param name="field">排序欄位</param>
        /// <param name="ascending">是否升序</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder OrderBy(string field, bool ascending = true);

        /// <summary>
        /// 設定包含資源
        /// </summary>
        /// <param name="includes">包含的資源</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder Include(params string[] includes);

        /// <summary>
        /// 設定反向包含資源
        /// </summary>
        /// <param name="revIncludes">反向包含的資源</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder RevInclude(params string[] revIncludes);

        /// <summary>
        /// 啟用相容性檢查
        /// </summary>
        /// <param name="enabled">是否啟用</param>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder WithCompatibilityCheck(bool enabled = true);

        /// <summary>
        /// 建構查詢
        /// </summary>
        /// <returns>查詢建構結果</returns>
        Task<QueryBuildResult> BuildAsync();

        /// <summary>
        /// 驗證查詢
        /// </summary>
        /// <returns>驗證結果</returns>
        Task<QueryValidationResult> ValidateAsync();

        /// <summary>
        /// 取得替代查詢建議
        /// </summary>
        /// <returns>替代查詢列表</returns>
        Task<List<QueryAlternative>> GetAlternativesAsync();

        /// <summary>
        /// 重設建構器
        /// </summary>
        /// <returns>查詢建構器</returns>
        IVersionAwareQueryBuilder Reset();
    }

    /// <summary>
    /// 查詢建構結果
    /// </summary>
    public class QueryBuildResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 查詢字串
        /// </summary>
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// 完整 URL
        /// </summary>
        public string FullUrl { get; set; } = string.Empty;

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 警告訊息
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// 查詢元數據
        /// </summary>
        public QueryMetadata Metadata { get; set; } = new();

        /// <summary>
        /// 建立成功結果
        /// </summary>
        /// <param name="query">查詢字串</param>
        /// <param name="fullUrl">完整 URL</param>
        /// <returns>成功結果</returns>
        public static QueryBuildResult Success(string query, string fullUrl)
        {
            return new QueryBuildResult
            {
                IsSuccess = true,
                Query = query,
                FullUrl = fullUrl
            };
        }

        /// <summary>
        /// 建立失敗結果
        /// </summary>
        /// <param name="errorMessage">錯誤訊息</param>
        /// <returns>失敗結果</returns>
        public static QueryBuildResult Failure(string errorMessage)
        {
            return new QueryBuildResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// 查詢替代方案
    /// </summary>
    public class QueryAlternative
    {
        /// <summary>
        /// 替代查詢
        /// </summary>
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 相容性分數 (0-100)
        /// </summary>
        public int CompatibilityScore { get; set; }

        /// <summary>
        /// 建議原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// 查詢元數據
    /// </summary>
    public class QueryMetadata
    {
        /// <summary>
        /// FHIR 版本
        /// </summary>
        public FhirVersion Version { get; set; }

        /// <summary>
        /// 資源類型
        /// </summary>
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// 伺服器 URL
        /// </summary>
        public string ServerUrl { get; set; } = string.Empty;

        /// <summary>
        /// 參數數量
        /// </summary>
        public int ParameterCount { get; set; }

        /// <summary>
        /// 預估複雜度 (1-10)
        /// </summary>
        public int EstimatedComplexity { get; set; }

        /// <summary>
        /// 是否經過相容性檢查
        /// </summary>
        public bool CompatibilityChecked { get; set; }

        /// <summary>
        /// 建構時間
        /// </summary>
        public DateTime BuildTime { get; set; } = DateTime.UtcNow;
    }
}
