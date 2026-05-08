using Fhir.QueryBuilder.Common;

namespace Fhir.QueryBuilder.QueryBuilders.Advanced
{
    /// <summary>
    /// 進階搜尋參數建構器介面
    /// </summary>
    public interface IAdvancedSearchParameterBuilder
    {
        /// <summary>
        /// 新增基本搜尋參數
        /// </summary>
        /// <param name="name">參數名稱</param>
        /// <param name="value">參數值</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddParameter(string name, string value);

        /// <summary>
        /// 新增帶修飾符的搜尋參數
        /// </summary>
        /// <param name="name">參數名稱</param>
        /// <param name="modifier">修飾符</param>
        /// <param name="value">參數值</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddParameter(string name, SearchModifier modifier, string value);

        /// <summary>
        /// 新增帶前綴的搜尋參數
        /// </summary>
        /// <param name="name">參數名稱</param>
        /// <param name="prefix">前綴</param>
        /// <param name="value">參數值</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddParameter(string name, SearchPrefix prefix, string value);

        /// <summary>
        /// 新增 Composite 搜尋參數
        /// </summary>
        /// <param name="name">參數名稱</param>
        /// <param name="components">組件值</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddCompositeParameter(string name, params string[] components);

        /// <summary>
        /// 新增 Chaining 搜尋參數
        /// </summary>
        /// <param name="chain">鏈式路徑，如 "patient.name"</param>
        /// <param name="value">參數值</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddChainParameter(string chain, string value);

        /// <summary>
        /// 新增 Reverse Chaining 搜尋參數
        /// </summary>
        /// <param name="resourceType">資源類型</param>
        /// <param name="searchParam">搜尋參數</param>
        /// <param name="value">參數值</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddReverseChainParameter(string resourceType, string searchParam, string value);

        /// <summary>
        /// 新增 _include 參數
        /// </summary>
        /// <param name="include">包含規則，如 "Patient:organization"</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddInclude(string include);

        /// <summary>
        /// 新增 _revinclude 參數
        /// </summary>
        /// <param name="revinclude">反向包含規則，如 "Observation:patient"</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddRevInclude(string revinclude);

        /// <summary>
        /// 設定排序
        /// </summary>
        /// <param name="parameter">排序參數</param>
        /// <param name="descending">是否降序</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder SetSort(string parameter, bool descending = false);

        /// <summary>
        /// 設定分頁
        /// </summary>
        /// <param name="count">每頁數量</param>
        /// <param name="offset">偏移量</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder SetPaging(int count, int offset = 0);

        /// <summary>
        /// 設定總數控制
        /// </summary>
        /// <param name="totalMode">總數模式</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder SetTotal(TotalMode totalMode);

        /// <summary>
        /// 設定摘要模式
        /// </summary>
        /// <param name="summaryMode">摘要模式</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder SetSummary(SummaryMode summaryMode);

        /// <summary>
        /// 設定元素選擇
        /// </summary>
        /// <param name="elements">要包含的元素</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder SetElements(params string[] elements);

        /// <summary>
        /// 新增 _filter 參數
        /// </summary>
        /// <param name="filter">過濾表達式</param>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder AddFilter(string filter);

        /// <summary>
        /// 建構查詢字串
        /// </summary>
        /// <returns>查詢字串</returns>
        string Build();

        /// <summary>
        /// 驗證參數
        /// </summary>
        /// <param name="resourceType">資源類型</param>
        /// <param name="version">FHIR 版本</param>
        /// <returns>驗證結果</returns>
        Task<SearchParameterValidationResult> ValidateAsync(string resourceType, FhirVersion version);

        /// <summary>
        /// 重設建構器
        /// </summary>
        /// <returns>建構器實例</returns>
        IAdvancedSearchParameterBuilder Reset();
    }

    /// <summary>
    /// 搜尋修飾符
    /// </summary>
    public enum SearchModifier
    {
        /// <summary>
        /// 缺失值檢查
        /// </summary>
        Missing,

        /// <summary>
        /// 精確匹配
        /// </summary>
        Exact,

        /// <summary>
        /// 包含匹配
        /// </summary>
        Contains,

        /// <summary>
        /// 文字匹配
        /// </summary>
        Text,

        /// <summary>
        /// 否定匹配
        /// </summary>
        Not,

        /// <summary>
        /// 上級匹配（階層）
        /// </summary>
        Above,

        /// <summary>
        /// 下級匹配（階層）
        /// </summary>
        Below,

        /// <summary>
        /// 值集包含
        /// </summary>
        In,

        /// <summary>
        /// 值集不包含
        /// </summary>
        NotIn,

        /// <summary>
        /// 類型匹配
        /// </summary>
        OfType,

        /// <summary>
        /// 識別符匹配
        /// </summary>
        Identifier,

        /// <summary>
        /// 進階文字匹配
        /// </summary>
        TextAdvanced,

        /// <summary>
        /// 代碼文字匹配
        /// </summary>
        CodeText,

        /// <summary>
        /// 迭代修飾符
        /// </summary>
        Iterate
    }

    /// <summary>
    /// 搜尋前綴
    /// </summary>
    public enum SearchPrefix
    {
        /// <summary>
        /// 等於
        /// </summary>
        Equal,

        /// <summary>
        /// 不等於
        /// </summary>
        NotEqual,

        /// <summary>
        /// 大於
        /// </summary>
        GreaterThan,

        /// <summary>
        /// 小於
        /// </summary>
        LessThan,

        /// <summary>
        /// 大於等於
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// 小於等於
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// 開始於
        /// </summary>
        StartAfter,

        /// <summary>
        /// 結束於
        /// </summary>
        EndBefore,

        /// <summary>
        /// 近似
        /// </summary>
        Approximate
    }

    /// <summary>
    /// 總數模式
    /// </summary>
    public enum TotalMode
    {
        /// <summary>
        /// 無總數
        /// </summary>
        None,

        /// <summary>
        /// 估算總數
        /// </summary>
        Estimate,

        /// <summary>
        /// 精確總數
        /// </summary>
        Accurate
    }

    /// <summary>
    /// 摘要模式
    /// </summary>
    public enum SummaryMode
    {
        /// <summary>
        /// 完整資源
        /// </summary>
        False,

        /// <summary>
        /// 摘要資源
        /// </summary>
        True,

        /// <summary>
        /// 僅文字
        /// </summary>
        Text,

        /// <summary>
        /// 僅資料
        /// </summary>
        Data,

        /// <summary>
        /// 計數
        /// </summary>
        Count
    }

    /// <summary>
    /// 搜尋參數驗證結果
    /// </summary>
    public class SearchParameterValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 驗證錯誤
        /// </summary>
        public List<ValidationError> Errors { get; set; } = new();

        /// <summary>
        /// 驗證警告
        /// </summary>
        public List<ValidationWarning> Warnings { get; set; } = new();

        /// <summary>
        /// 建議的修正
        /// </summary>
        public List<ValidationSuggestion> Suggestions { get; set; } = new();
    }

    /// <summary>
    /// 驗證錯誤
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// 參數名稱
        /// </summary>
        public string ParameterName { get; set; } = string.Empty;

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 錯誤類型
        /// </summary>
        public ValidationErrorType Type { get; set; }
    }

    /// <summary>
    /// 驗證警告
    /// </summary>
    public class ValidationWarning
    {
        /// <summary>
        /// 參數名稱
        /// </summary>
        public string ParameterName { get; set; } = string.Empty;

        /// <summary>
        /// 警告訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 警告類型
        /// </summary>
        public ValidationWarningType Type { get; set; }
    }

    /// <summary>
    /// 驗證建議
    /// </summary>
    public class ValidationSuggestion
    {
        /// <summary>
        /// 原始參數
        /// </summary>
        public string OriginalParameter { get; set; } = string.Empty;

        /// <summary>
        /// 建議參數
        /// </summary>
        public string SuggestedParameter { get; set; } = string.Empty;

        /// <summary>
        /// 建議原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// 信心分數 (0-100)
        /// </summary>
        public int ConfidenceScore { get; set; }
    }

    /// <summary>
    /// 驗證錯誤類型
    /// </summary>
    public enum ValidationErrorType
    {
        /// <summary>
        /// 未知參數
        /// </summary>
        UnknownParameter,

        /// <summary>
        /// 不支援的修飾符
        /// </summary>
        UnsupportedModifier,

        /// <summary>
        /// 不支援的前綴
        /// </summary>
        UnsupportedPrefix,

        /// <summary>
        /// 無效的參數值
        /// </summary>
        InvalidValue,

        /// <summary>
        /// 無效的組合
        /// </summary>
        InvalidCombination,

        /// <summary>
        /// 語法錯誤
        /// </summary>
        SyntaxError
    }

    /// <summary>
    /// 驗證警告類型
    /// </summary>
    public enum ValidationWarningType
    {
        /// <summary>
        /// 效能警告
        /// </summary>
        Performance,

        /// <summary>
        /// 相容性警告
        /// </summary>
        Compatibility,

        /// <summary>
        /// 最佳實踐建議
        /// </summary>
        BestPractice,

        /// <summary>
        /// 廢棄功能
        /// </summary>
        Deprecated
    }
}
