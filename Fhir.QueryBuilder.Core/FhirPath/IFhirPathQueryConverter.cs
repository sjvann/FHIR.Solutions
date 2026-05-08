using Fhir.QueryBuilder.Common;

namespace Fhir.QueryBuilder.FhirPath
{
    /// <summary>
    /// FHIRPath 查詢轉換器介面
    /// </summary>
    public interface IFhirPathQueryConverter
    {
        /// <summary>
        /// 將 FHIRPath 表達式轉換為搜尋參數
        /// </summary>
        /// <param name="expression">FHIRPath 表達式</param>
        /// <param name="resourceType">資源類型</param>
        /// <param name="version">FHIR 版本</param>
        /// <returns>轉換結果</returns>
        Task<FhirPathConversionResult> ConvertToSearchParametersAsync(
            string expression, 
            string resourceType, 
            FhirVersion version);

        /// <summary>
        /// 驗證 FHIRPath 表達式
        /// </summary>
        /// <param name="expression">FHIRPath 表達式</param>
        /// <param name="resourceType">資源類型</param>
        /// <param name="version">FHIR 版本</param>
        /// <returns>驗證結果</returns>
        Task<FhirPathValidationResult> ValidateExpressionAsync(
            string expression, 
            string resourceType, 
            FhirVersion version);

        /// <summary>
        /// 取得支援的 FHIRPath 功能
        /// </summary>
        /// <returns>支援的功能列表</returns>
        FhirPathCapabilities GetSupportedCapabilities();

        /// <summary>
        /// 分析 FHIRPath 表達式
        /// </summary>
        /// <param name="expression">FHIRPath 表達式</param>
        /// <returns>分析結果</returns>
        FhirPathAnalysisResult AnalyzeExpression(string expression);
    }

    /// <summary>
    /// FHIRPath 轉換結果
    /// </summary>
    public class FhirPathConversionResult
    {
        /// <summary>
        /// 是否成功轉換
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// 轉換的搜尋參數
        /// </summary>
        public List<ConvertedSearchParameter> SearchParameters { get; set; } = new();

        /// <summary>
        /// 無法轉換的部分
        /// </summary>
        public List<string> UnsupportedExpressions { get; set; } = new();

        /// <summary>
        /// 轉換警告
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// 轉換建議
        /// </summary>
        public List<string> Suggestions { get; set; } = new();

        /// <summary>
        /// 原始表達式
        /// </summary>
        public string OriginalExpression { get; set; } = string.Empty;

        /// <summary>
        /// 轉換信心分數 (0-100)
        /// </summary>
        public int ConfidenceScore { get; set; }
    }

    /// <summary>
    /// 轉換的搜尋參數
    /// </summary>
    public class ConvertedSearchParameter
    {
        /// <summary>
        /// 參數名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 參數值
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 修飾符
        /// </summary>
        public string? Modifier { get; set; }

        /// <summary>
        /// 前綴
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// 對應的 FHIRPath 片段
        /// </summary>
        public string SourceExpression { get; set; } = string.Empty;

        /// <summary>
        /// 轉換信心分數 (0-100)
        /// </summary>
        public int ConfidenceScore { get; set; } = 100;

        /// <summary>
        /// 轉換說明
        /// </summary>
        public string? ConversionNote { get; set; }
    }

    /// <summary>
    /// FHIRPath 驗證結果
    /// </summary>
    public class FhirPathValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 語法錯誤
        /// </summary>
        public List<FhirPathError> SyntaxErrors { get; set; } = new();

        /// <summary>
        /// 語義錯誤
        /// </summary>
        public List<FhirPathError> SemanticErrors { get; set; } = new();

        /// <summary>
        /// 警告
        /// </summary>
        public List<FhirPathWarning> Warnings { get; set; } = new();

        /// <summary>
        /// 建議
        /// </summary>
        public List<string> Suggestions { get; set; } = new();
    }

    /// <summary>
    /// FHIRPath 錯誤
    /// </summary>
    public class FhirPathError
    {
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 錯誤位置（字元索引）
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 錯誤長度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 錯誤類型
        /// </summary>
        public FhirPathErrorType Type { get; set; }
    }

    /// <summary>
    /// FHIRPath 警告
    /// </summary>
    public class FhirPathWarning
    {
        /// <summary>
        /// 警告訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 警告位置
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 警告類型
        /// </summary>
        public FhirPathWarningType Type { get; set; }
    }

    /// <summary>
    /// FHIRPath 錯誤類型
    /// </summary>
    public enum FhirPathErrorType
    {
        /// <summary>
        /// 語法錯誤
        /// </summary>
        Syntax,

        /// <summary>
        /// 未知函數
        /// </summary>
        UnknownFunction,

        /// <summary>
        /// 未知屬性
        /// </summary>
        UnknownProperty,

        /// <summary>
        /// 類型不匹配
        /// </summary>
        TypeMismatch,

        /// <summary>
        /// 無效操作
        /// </summary>
        InvalidOperation
    }

    /// <summary>
    /// FHIRPath 警告類型
    /// </summary>
    public enum FhirPathWarningType
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
        BestPractice
    }

    /// <summary>
    /// FHIRPath 分析結果
    /// </summary>
    public class FhirPathAnalysisResult
    {
        /// <summary>
        /// 使用的屬性
        /// </summary>
        public List<string> UsedProperties { get; set; } = new();

        /// <summary>
        /// 使用的函數
        /// </summary>
        public List<string> UsedFunctions { get; set; } = new();

        /// <summary>
        /// 使用的操作符
        /// </summary>
        public List<string> UsedOperators { get; set; } = new();

        /// <summary>
        /// 複雜度分數 (1-10)
        /// </summary>
        public int ComplexityScore { get; set; }

        /// <summary>
        /// 是否可轉換為搜尋參數
        /// </summary>
        public bool IsConvertibleToSearch { get; set; }

        /// <summary>
        /// 轉換難度
        /// </summary>
        public ConversionDifficulty ConversionDifficulty { get; set; }

        /// <summary>
        /// 分析備註
        /// </summary>
        public List<string> Notes { get; set; } = new();
    }

    /// <summary>
    /// 轉換難度
    /// </summary>
    public enum ConversionDifficulty
    {
        /// <summary>
        /// 簡單 - 直接對應
        /// </summary>
        Easy,

        /// <summary>
        /// 中等 - 需要一些轉換
        /// </summary>
        Medium,

        /// <summary>
        /// 困難 - 複雜轉換
        /// </summary>
        Hard,

        /// <summary>
        /// 不可能 - 無法轉換
        /// </summary>
        Impossible
    }

    /// <summary>
    /// FHIRPath 功能支援
    /// </summary>
    public class FhirPathCapabilities
    {
        /// <summary>
        /// 支援的函數
        /// </summary>
        public List<string> SupportedFunctions { get; set; } = new();

        /// <summary>
        /// 支援的操作符
        /// </summary>
        public List<string> SupportedOperators { get; set; } = new();

        /// <summary>
        /// 支援的資料類型
        /// </summary>
        public List<string> SupportedDataTypes { get; set; } = new();

        /// <summary>
        /// 最大表達式複雜度
        /// </summary>
        public int MaxComplexity { get; set; } = 10;

        /// <summary>
        /// 是否支援巢狀表達式
        /// </summary>
        public bool SupportsNestedExpressions { get; set; } = true;

        /// <summary>
        /// 是否支援條件表達式
        /// </summary>
        public bool SupportsConditionalExpressions { get; set; } = true;

        /// <summary>
        /// 版本資訊
        /// </summary>
        public string Version { get; set; } = "1.0";
    }
}
