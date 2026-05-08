using Fhir.QueryBuilder.Common;

namespace Fhir.QueryBuilder.Results
{
    /// <summary>
    /// 搜尋結果處理器介面
    /// </summary>
    public interface ISearchResultProcessor
    {
        /// <summary>
        /// 處理搜尋結果
        /// </summary>
        /// <param name="searchResult">原始搜尋結果</param>
        /// <param name="options">處理選項</param>
        /// <returns>處理後的結果</returns>
        Task<ProcessedSearchResult> ProcessResultAsync(string searchResult, SearchResultProcessingOptions? options = null);

        /// <summary>
        /// 提取分頁資訊
        /// </summary>
        /// <param name="searchResult">搜尋結果</param>
        /// <returns>分頁資訊</returns>
        PaginationInfo ExtractPaginationInfo(string searchResult);

        /// <summary>
        /// 提取包含的資源
        /// </summary>
        /// <param name="searchResult">搜尋結果</param>
        /// <returns>包含的資源</returns>
        IncludedResources ExtractIncludedResources(string searchResult);

        /// <summary>
        /// 提取操作結果
        /// </summary>
        /// <param name="searchResult">搜尋結果</param>
        /// <returns>操作結果</returns>
        OperationOutcome? ExtractOperationOutcome(string searchResult);

        /// <summary>
        /// 驗證搜尋結果格式
        /// </summary>
        /// <param name="searchResult">搜尋結果</param>
        /// <returns>驗證結果</returns>
        SearchResultValidation ValidateResult(string searchResult);

        /// <summary>
        /// 轉換為指定格式
        /// </summary>
        /// <param name="searchResult">搜尋結果</param>
        /// <param name="format">目標格式</param>
        /// <returns>轉換後的結果</returns>
        Task<string> ConvertFormatAsync(string searchResult, ResultFormat format);
    }

    /// <summary>
    /// 搜尋結果處理選項
    /// </summary>
    public class SearchResultProcessingOptions
    {
        /// <summary>
        /// 是否包含原始 JSON
        /// </summary>
        public bool IncludeRawJson { get; set; } = false;

        /// <summary>
        /// 是否解析包含的資源
        /// </summary>
        public bool ParseIncludedResources { get; set; } = true;

        /// <summary>
        /// 是否提取分頁資訊
        /// </summary>
        public bool ExtractPagination { get; set; } = true;

        /// <summary>
        /// 是否驗證結果
        /// </summary>
        public bool ValidateResult { get; set; } = true;

        /// <summary>
        /// 最大處理的資源數量
        /// </summary>
        public int MaxResourceCount { get; set; } = 1000;

        /// <summary>
        /// 是否提取統計資訊
        /// </summary>
        public bool ExtractStatistics { get; set; } = true;

        /// <summary>
        /// 自訂處理器
        /// </summary>
        public List<ICustomResultProcessor> CustomProcessors { get; set; } = new();
    }

    /// <summary>
    /// 處理後的搜尋結果
    /// </summary>
    public class ProcessedSearchResult
    {
        /// <summary>
        /// 資源類型
        /// </summary>
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// 總數量
        /// </summary>
        public int? Total { get; set; }

        /// <summary>
        /// 資源列表
        /// </summary>
        public List<FhirResource> Resources { get; set; } = new();

        /// <summary>
        /// 包含的資源
        /// </summary>
        public IncludedResources IncludedResources { get; set; } = new();

        /// <summary>
        /// 分頁資訊
        /// </summary>
        public PaginationInfo Pagination { get; set; } = new();

        /// <summary>
        /// 操作結果
        /// </summary>
        public OperationOutcome? OperationOutcome { get; set; }

        /// <summary>
        /// 搜尋統計
        /// </summary>
        public SearchStatistics Statistics { get; set; } = new();

        /// <summary>
        /// 原始 JSON（如果請求）
        /// </summary>
        public string? RawJson { get; set; }

        /// <summary>
        /// 處理時間
        /// </summary>
        public TimeSpan ProcessingTime { get; set; }

        /// <summary>
        /// 處理警告
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// 處理錯誤
        /// </summary>
        public List<string> Errors { get; set; } = new();
    }

    /// <summary>
    /// FHIR 資源
    /// </summary>
    public class FhirResource
    {
        /// <summary>
        /// 資源 ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 資源類型
        /// </summary>
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// 版本 ID
        /// </summary>
        public string? VersionId { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// 資源內容（JSON）
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 搜尋模式
        /// </summary>
        public SearchEntryMode? SearchMode { get; set; }

        /// <summary>
        /// 搜尋分數
        /// </summary>
        public decimal? SearchScore { get; set; }

        /// <summary>
        /// 資源 URL
        /// </summary>
        public string? FullUrl { get; set; }
    }

    /// <summary>
    /// 包含的資源
    /// </summary>
    public class IncludedResources
    {
        /// <summary>
        /// 按資源類型分組的包含資源
        /// </summary>
        public Dictionary<string, List<FhirResource>> ResourcesByType { get; set; } = new();

        /// <summary>
        /// 所有包含的資源
        /// </summary>
        public List<FhirResource> AllResources => ResourcesByType.Values.SelectMany(r => r).ToList();

        /// <summary>
        /// 包含資源的總數
        /// </summary>
        public int TotalCount => AllResources.Count;
    }

    /// <summary>
    /// 分頁資訊
    /// </summary>
    public class PaginationInfo
    {
        /// <summary>
        /// 第一頁連結
        /// </summary>
        public string? FirstPageUrl { get; set; }

        /// <summary>
        /// 上一頁連結
        /// </summary>
        public string? PreviousPageUrl { get; set; }

        /// <summary>
        /// 下一頁連結
        /// </summary>
        public string? NextPageUrl { get; set; }

        /// <summary>
        /// 最後一頁連結
        /// </summary>
        public string? LastPageUrl { get; set; }

        /// <summary>
        /// 自身連結
        /// </summary>
        public string? SelfUrl { get; set; }

        /// <summary>
        /// 當前頁面大小
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// 當前頁碼（估算）
        /// </summary>
        public int? CurrentPage { get; set; }

        /// <summary>
        /// 總頁數（估算）
        /// </summary>
        public int? TotalPages { get; set; }

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        public bool HasNextPage => !string.IsNullOrEmpty(NextPageUrl);

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        public bool HasPreviousPage => !string.IsNullOrEmpty(PreviousPageUrl);
    }

    /// <summary>
    /// 操作結果
    /// </summary>
    public class OperationOutcome
    {
        /// <summary>
        /// 問題列表
        /// </summary>
        public List<OperationOutcomeIssue> Issues { get; set; } = new();

        /// <summary>
        /// 是否有錯誤
        /// </summary>
        public bool HasErrors => Issues.Any(i => i.Severity == IssueSeverity.Error || i.Severity == IssueSeverity.Fatal);

        /// <summary>
        /// 是否有警告
        /// </summary>
        public bool HasWarnings => Issues.Any(i => i.Severity == IssueSeverity.Warning);
    }

    /// <summary>
    /// 操作結果問題
    /// </summary>
    public class OperationOutcomeIssue
    {
        /// <summary>
        /// 嚴重程度
        /// </summary>
        public IssueSeverity Severity { get; set; }

        /// <summary>
        /// 問題代碼
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 詳細訊息
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// 診斷訊息
        /// </summary>
        public string? Diagnostics { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public List<string> Location { get; set; } = new();
    }

    /// <summary>
    /// 問題嚴重程度
    /// </summary>
    public enum IssueSeverity
    {
        /// <summary>
        /// 致命錯誤
        /// </summary>
        Fatal,

        /// <summary>
        /// 錯誤
        /// </summary>
        Error,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// 資訊
        /// </summary>
        Information
    }

    /// <summary>
    /// 搜尋條目模式
    /// </summary>
    public enum SearchEntryMode
    {
        /// <summary>
        /// 匹配
        /// </summary>
        Match,

        /// <summary>
        /// 包含
        /// </summary>
        Include,

        /// <summary>
        /// 結果
        /// </summary>
        Outcome
    }

    /// <summary>
    /// 搜尋統計
    /// </summary>
    public class SearchStatistics
    {
        /// <summary>
        /// 匹配的資源數量
        /// </summary>
        public int MatchedResourceCount { get; set; }

        /// <summary>
        /// 包含的資源數量
        /// </summary>
        public int IncludedResourceCount { get; set; }

        /// <summary>
        /// 按資源類型分組的數量
        /// </summary>
        public Dictionary<string, int> ResourceCountByType { get; set; } = new();

        /// <summary>
        /// 平均搜尋分數
        /// </summary>
        public decimal? AverageSearchScore { get; set; }

        /// <summary>
        /// 最高搜尋分數
        /// </summary>
        public decimal? MaxSearchScore { get; set; }

        /// <summary>
        /// 最低搜尋分數
        /// </summary>
        public decimal? MinSearchScore { get; set; }
    }

    /// <summary>
    /// 搜尋結果驗證
    /// </summary>
    public class SearchResultValidation
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 驗證錯誤
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// 驗證警告
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// 檢測到的格式
        /// </summary>
        public ResultFormat DetectedFormat { get; set; }

        /// <summary>
        /// FHIR 版本
        /// </summary>
        public FhirVersion? DetectedVersion { get; set; }
    }

    /// <summary>
    /// 結果格式
    /// </summary>
    public enum ResultFormat
    {
        /// <summary>
        /// JSON
        /// </summary>
        Json,

        /// <summary>
        /// XML
        /// </summary>
        Xml,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown
    }

    /// <summary>
    /// 自訂結果處理器介面
    /// </summary>
    public interface ICustomResultProcessor
    {
        /// <summary>
        /// 處理器名稱
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 處理結果
        /// </summary>
        /// <param name="result">處理中的結果</param>
        /// <param name="rawJson">原始 JSON</param>
        /// <returns>處理任務</returns>
        Task ProcessAsync(ProcessedSearchResult result, string rawJson);
    }
}
