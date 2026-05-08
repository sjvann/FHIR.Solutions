using Fhir.QueryBuilder.Common;
using Fhir.QueryBuilder.Models;

namespace Fhir.QueryBuilder.Services.Interfaces
{
    /// <summary>
    /// FHIR 查詢服務介面
    /// </summary>
    public interface IFhirQueryService
    {
        // 原有方法（保持向後相容）
        Task<Fhir.Resources.R5.CapabilityStatement?> ConnectToServerAsync(string baseUrl, CancellationToken cancellationToken = default);
        Task<string?> ExecuteQueryAsync(string queryUrl, string? token = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>?> GetSupportedResourcesAsync(CancellationToken cancellationToken = default);
        Task<string[]?> GetSearchIncludeAsync(string resourceName, CancellationToken cancellationToken = default);
        Task<string[]?> GetSearchRevIncludeAsync(string resourceName, CancellationToken cancellationToken = default);
        Task<IEnumerable<SearchParamModel>?> GetSearchParametersAsync(string resourceName, CancellationToken cancellationToken = default);
        bool IsConnected { get; }
        string? BaseUrl { get; }
        bool SupportOAuth { get; }
        string? AuthorizeUrl { get; }
        string? TokenUrl { get; }

        // 新增方法（新架構）
        /// <summary>
        /// 執行結構化查詢
        /// </summary>
        /// <param name="serverUrl">伺服器 URL</param>
        /// <param name="query">查詢字串</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>查詢結果</returns>
        Task<FhirQueryResult> ExecuteStructuredQueryAsync(string serverUrl, string query, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得能力聲明（新版本）
        /// </summary>
        /// <param name="serverUrl">伺服器 URL</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>能力聲明</returns>
        Task<Fhir.Resources.R5.CapabilityStatement?> GetCapabilityStatementAsync(string serverUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// 測試伺服器連線
        /// </summary>
        /// <param name="serverUrl">伺服器 URL</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>連線測試結果</returns>
        Task<ServerConnectionResult> TestConnectionAsync(string serverUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得資源
        /// </summary>
        /// <param name="serverUrl">伺服器 URL</param>
        /// <param name="resourceType">資源類型</param>
        /// <param name="resourceId">資源 ID</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>資源結果</returns>
        Task<FhirResourceResult> GetResourceAsync(string serverUrl, string resourceType, string resourceId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// FHIR 查詢結果
    /// </summary>
    public class FhirQueryResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// HTTP 狀態碼
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 回應內容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 回應時間（毫秒）
        /// </summary>
        public long ResponseTimeMs { get; set; }

        /// <summary>
        /// 資源數量
        /// </summary>
        public int ResourceCount { get; set; }

        /// <summary>
        /// 總數量（如果有分頁）
        /// </summary>
        public int? TotalCount { get; set; }

        /// <summary>
        /// 下一頁連結
        /// </summary>
        public string? NextPageUrl { get; set; }

        /// <summary>
        /// 上一頁連結
        /// </summary>
        public string? PreviousPageUrl { get; set; }

        /// <summary>
        /// 回應標頭
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new();
    }

    /// <summary>
    /// 伺服器連線結果
    /// </summary>
    public class ServerConnectionResult
    {
        /// <summary>
        /// 是否可連線
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// 回應時間（毫秒）
        /// </summary>
        public long ResponseTimeMs { get; set; }

        /// <summary>
        /// FHIR 版本
        /// </summary>
        public FhirVersion? FhirVersion { get; set; }

        /// <summary>
        /// 伺服器軟體
        /// </summary>
        public string? ServerSoftware { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// HTTP 狀態碼
        /// </summary>
        public int StatusCode { get; set; }
    }

    /// <summary>
    /// FHIR 資源結果
    /// </summary>
    public class FhirResourceResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// HTTP 狀態碼
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 資源內容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 資源類型
        /// </summary>
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// 資源 ID
        /// </summary>
        public string ResourceId { get; set; } = string.Empty;

        /// <summary>
        /// 版本 ID
        /// </summary>
        public string? VersionId { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 回應時間（毫秒）
        /// </summary>
        public long ResponseTimeMs { get; set; }
    }
}
