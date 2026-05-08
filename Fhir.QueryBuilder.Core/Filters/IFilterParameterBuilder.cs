using Fhir.QueryBuilder.Common;

namespace Fhir.QueryBuilder.Filters
{
    /// <summary>
    /// _filter 參數建構器介面
    /// </summary>
    public interface IFilterParameterBuilder
    {
        /// <summary>
        /// 新增等於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder Equal(string path, string value);

        /// <summary>
        /// 新增不等於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder NotEqual(string path, string value);

        /// <summary>
        /// 新增大於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder GreaterThan(string path, string value);

        /// <summary>
        /// 新增小於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder LessThan(string path, string value);

        /// <summary>
        /// 新增大於等於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder GreaterThanOrEqual(string path, string value);

        /// <summary>
        /// 新增小於等於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder LessThanOrEqual(string path, string value);

        /// <summary>
        /// 新增包含條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder Contains(string path, string value);

        /// <summary>
        /// 新增開始於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder StartsWith(string path, string value);

        /// <summary>
        /// 新增結束於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder EndsWith(string path, string value);

        /// <summary>
        /// 新增存在條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder Exists(string path);

        /// <summary>
        /// 新增不存在條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder NotExists(string path);

        /// <summary>
        /// 新增 IN 條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="values">值列表</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder In(string path, params string[] values);

        /// <summary>
        /// 新增 NOT IN 條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="values">值列表</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder NotIn(string path, params string[] values);

        /// <summary>
        /// 新增 AND 邏輯
        /// </summary>
        /// <param name="condition">條件建構器</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder And(Action<IFilterParameterBuilder> condition);

        /// <summary>
        /// 新增 OR 邏輯
        /// </summary>
        /// <param name="condition">條件建構器</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder Or(Action<IFilterParameterBuilder> condition);

        /// <summary>
        /// 新增 NOT 邏輯
        /// </summary>
        /// <param name="condition">條件建構器</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder Not(Action<IFilterParameterBuilder> condition);

        /// <summary>
        /// 新增群組條件
        /// </summary>
        /// <param name="condition">條件建構器</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder Group(Action<IFilterParameterBuilder> condition);

        /// <summary>
        /// 新增自訂條件
        /// </summary>
        /// <param name="expression">自訂表達式</param>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder Custom(string expression);

        /// <summary>
        /// 建構 _filter 參數值
        /// </summary>
        /// <returns>_filter 參數值</returns>
        string Build();

        /// <summary>
        /// 驗證 filter 表達式
        /// </summary>
        /// <param name="resourceType">資源類型</param>
        /// <param name="version">FHIR 版本</param>
        /// <returns>驗證結果</returns>
        Task<FilterValidationResult> ValidateAsync(string resourceType, FhirVersion version);

        /// <summary>
        /// 重設建構器
        /// </summary>
        /// <returns>建構器實例</returns>
        IFilterParameterBuilder Reset();

        /// <summary>
        /// 取得表達式複雜度
        /// </summary>
        int Complexity { get; }
    }

    /// <summary>
    /// Filter 驗證結果
    /// </summary>
    public class FilterValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public List<FilterError> Errors { get; set; } = new();

        /// <summary>
        /// 警告訊息
        /// </summary>
        public List<FilterWarning> Warnings { get; set; } = new();

        /// <summary>
        /// 建議
        /// </summary>
        public List<string> Suggestions { get; set; } = new();

        /// <summary>
        /// 複雜度分數
        /// </summary>
        public int ComplexityScore { get; set; }
    }

    /// <summary>
    /// Filter 錯誤
    /// </summary>
    public class FilterError
    {
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 錯誤位置
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 錯誤類型
        /// </summary>
        public FilterErrorType Type { get; set; }

        /// <summary>
        /// 相關路徑
        /// </summary>
        public string? Path { get; set; }
    }

    /// <summary>
    /// Filter 警告
    /// </summary>
    public class FilterWarning
    {
        /// <summary>
        /// 警告訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 警告類型
        /// </summary>
        public FilterWarningType Type { get; set; }

        /// <summary>
        /// 相關路徑
        /// </summary>
        public string? Path { get; set; }
    }

    /// <summary>
    /// Filter 錯誤類型
    /// </summary>
    public enum FilterErrorType
    {
        /// <summary>
        /// 語法錯誤
        /// </summary>
        Syntax,

        /// <summary>
        /// 未知路徑
        /// </summary>
        UnknownPath,

        /// <summary>
        /// 類型不匹配
        /// </summary>
        TypeMismatch,

        /// <summary>
        /// 無效操作
        /// </summary>
        InvalidOperation,

        /// <summary>
        /// 不支援的功能
        /// </summary>
        UnsupportedFeature
    }

    /// <summary>
    /// Filter 警告類型
    /// </summary>
    public enum FilterWarningType
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
        /// 最佳實踐
        /// </summary>
        BestPractice,

        /// <summary>
        /// 複雜度警告
        /// </summary>
        Complexity
    }

    /// <summary>
    /// Filter 操作符
    /// </summary>
    public enum FilterOperator
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
        /// 包含
        /// </summary>
        Contains,

        /// <summary>
        /// 開始於
        /// </summary>
        StartsWith,

        /// <summary>
        /// 結束於
        /// </summary>
        EndsWith,

        /// <summary>
        /// 存在
        /// </summary>
        Exists,

        /// <summary>
        /// 不存在
        /// </summary>
        NotExists,

        /// <summary>
        /// 在列表中
        /// </summary>
        In,

        /// <summary>
        /// 不在列表中
        /// </summary>
        NotIn
    }

    /// <summary>
    /// Filter 邏輯操作符
    /// </summary>
    public enum FilterLogicalOperator
    {
        /// <summary>
        /// 且
        /// </summary>
        And,

        /// <summary>
        /// 或
        /// </summary>
        Or,

        /// <summary>
        /// 非
        /// </summary>
        Not
    }

    /// <summary>
    /// Filter 條件
    /// </summary>
    public class FilterCondition
    {
        /// <summary>
        /// 元素路徑
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 操作符
        /// </summary>
        public FilterOperator Operator { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// 值列表（用於 IN/NOT IN）
        /// </summary>
        public List<string> Values { get; set; } = new();

        /// <summary>
        /// 是否為群組條件
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// 子條件（用於群組）
        /// </summary>
        public List<FilterCondition> SubConditions { get; set; } = new();

        /// <summary>
        /// 邏輯操作符（用於連接條件）
        /// </summary>
        public FilterLogicalOperator? LogicalOperator { get; set; }

        /// <summary>
        /// 自訂表達式
        /// </summary>
        public string? CustomExpression { get; set; }
    }
}
