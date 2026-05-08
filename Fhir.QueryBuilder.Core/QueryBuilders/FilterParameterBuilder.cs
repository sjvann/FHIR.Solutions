using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;
using ValidationResult = Fhir.QueryBuilder.Services.Interfaces.ValidationResult;

namespace Fhir.QueryBuilder.QueryBuilders
{
    /// <summary>
    /// _filter 參數建構器
    /// </summary>
    public class FilterParameterBuilder : ISearchParameterBuilder
    {
        private readonly ILogger<FilterParameterBuilder> _logger;

        public string ParameterType => "filter";

        public FilterParameterBuilder(ILogger<FilterParameterBuilder> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(string parameterType)
        {
            return string.Equals(parameterType, "filter", StringComparison.OrdinalIgnoreCase);
        }

        public string BuildParameter(SearchParameterContext context)
        {
            try
            {
                var filterExpression = GetFilterExpression(context);
                
                if (string.IsNullOrWhiteSpace(filterExpression))
                {
                    throw new ArgumentException("Filter expression cannot be empty");
                }

                var result = $"_filter={Uri.EscapeDataString(filterExpression)}";
                
                _logger.LogDebug("Built filter parameter: {Parameter}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building filter parameter");
                throw;
            }
        }

        public ValidationResult ValidateParameter(SearchParameterContext context)
        {
            var result = new ValidationResult { IsValid = true };

            try
            {
                var filterExpression = GetFilterExpression(context);

                if (string.IsNullOrWhiteSpace(filterExpression))
                {
                    result.AddError("Filter expression cannot be empty");
                    return result;
                }

                // 基本語法驗證
                var validationErrors = ValidateFilterSyntax(filterExpression);
                foreach (var error in validationErrors)
                {
                    result.AddError(error);
                }

                _logger.LogDebug("Validated filter parameter, Valid: {IsValid}", result.IsValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating filter parameter");
                result.AddError($"Validation error: {ex.Message}");
            }

            return result;
        }

        private string GetFilterExpression(SearchParameterContext context)
        {
            if (context.Values.TryGetValue("expression", out var expressionObj))
            {
                return expressionObj.ToString() ?? string.Empty;
            }

            if (context.Values.TryGetValue("filter", out var filterObj))
            {
                return filterObj.ToString() ?? string.Empty;
            }

            // 嘗試從多個條件建構表達式
            return BuildExpressionFromContext(context);
        }

        private string BuildExpressionFromContext(SearchParameterContext context)
        {
            var conditions = new List<string>();

            // 檢查各種條件類型
            foreach (var kvp in context.Values)
            {
                var key = kvp.Key.ToLowerInvariant();
                var value = kvp.Value.ToString() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(value)) continue;

                switch (key)
                {
                    case "equal":
                    case "eq":
                        conditions.Add($"{GetPath(context)} eq '{value}'");
                        break;
                    case "notequal":
                    case "ne":
                        conditions.Add($"{GetPath(context)} ne '{value}'");
                        break;
                    case "greaterthan":
                    case "gt":
                        conditions.Add($"{GetPath(context)} gt '{value}'");
                        break;
                    case "lessthan":
                    case "lt":
                        conditions.Add($"{GetPath(context)} lt '{value}'");
                        break;
                    case "contains":
                        conditions.Add($"{GetPath(context)} co '{value}'");
                        break;
                    case "startswith":
                        conditions.Add($"{GetPath(context)} sw '{value}'");
                        break;
                    case "endswith":
                        conditions.Add($"{GetPath(context)} ew '{value}'");
                        break;
                }
            }

            return string.Join(" and ", conditions);
        }

        private string GetPath(SearchParameterContext context)
        {
            if (context.Values.TryGetValue("path", out var pathObj))
            {
                return pathObj.ToString() ?? "value";
            }

            return context.ParameterName == "_filter" ? "value" : context.ParameterName;
        }

        private List<string> ValidateFilterSyntax(string filterExpression)
        {
            var errors = new List<string>();

            try
            {
                // 基本語法檢查
                if (filterExpression.Contains("''"))
                {
                    errors.Add("Empty string literals are not allowed");
                }

                // 檢查括號平衡
                var openParens = filterExpression.Count(c => c == '(');
                var closeParens = filterExpression.Count(c => c == ')');
                if (openParens != closeParens)
                {
                    errors.Add("Unbalanced parentheses in filter expression");
                }

                // 檢查引號平衡
                var quotes = filterExpression.Count(c => c == '\'');
                if (quotes % 2 != 0)
                {
                    errors.Add("Unbalanced quotes in filter expression");
                }

                // 檢查支援的操作符
                var supportedOperators = new[] { "eq", "ne", "gt", "lt", "ge", "le", "co", "sw", "ew", "and", "or", "not" };
                var words = filterExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var word in words)
                {
                    if (word.All(char.IsLetter) && word.Length > 1 && !supportedOperators.Contains(word.ToLowerInvariant()))
                    {
                        // 可能是不支援的操作符或函數
                        if (!IsValidPath(word))
                        {
                            errors.Add($"Unsupported operator or function: {word}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Syntax validation error: {ex.Message}");
            }

            return errors;
        }

        private bool IsValidPath(string word)
        {
            // 簡化的路徑驗證 - 檢查是否看起來像 FHIR 路徑
            return word.Contains('.') || 
                   char.IsLower(word[0]) || 
                   word.All(c => char.IsLetterOrDigit(c) || c == '_');
        }
    }

    /// <summary>
    /// Filter 參數建構器的擴展方法
    /// </summary>
    public static class FilterParameterExtensions
    {
        /// <summary>
        /// 建立簡單的等於條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>Filter 表達式</returns>
        public static string CreateEqualFilter(string path, string value)
        {
            return $"{path} eq '{value}'";
        }

        /// <summary>
        /// 建立包含條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="value">值</param>
        /// <returns>Filter 表達式</returns>
        public static string CreateContainsFilter(string path, string value)
        {
            return $"{path} co '{value}'";
        }

        /// <summary>
        /// 建立範圍條件
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns>Filter 表達式</returns>
        public static string CreateRangeFilter(string path, string minValue, string maxValue)
        {
            return $"{path} ge '{minValue}' and {path} le '{maxValue}'";
        }

        /// <summary>
        /// 建立 OR 條件
        /// </summary>
        /// <param name="conditions">條件列表</param>
        /// <returns>Filter 表達式</returns>
        public static string CreateOrFilter(params string[] conditions)
        {
            if (conditions.Length == 0) return string.Empty;
            if (conditions.Length == 1) return conditions[0];
            
            return $"({string.Join(" or ", conditions)})";
        }

        /// <summary>
        /// 建立 AND 條件
        /// </summary>
        /// <param name="conditions">條件列表</param>
        /// <returns>Filter 表達式</returns>
        public static string CreateAndFilter(params string[] conditions)
        {
            if (conditions.Length == 0) return string.Empty;
            if (conditions.Length == 1) return conditions[0];
            
            return string.Join(" and ", conditions);
        }

        /// <summary>
        /// 建立 NOT 條件
        /// </summary>
        /// <param name="condition">條件</param>
        /// <returns>Filter 表達式</returns>
        public static string CreateNotFilter(string condition)
        {
            return $"not ({condition})";
        }

        /// <summary>
        /// 建立存在性檢查
        /// </summary>
        /// <param name="path">元素路徑</param>
        /// <param name="exists">是否存在</param>
        /// <returns>Filter 表達式</returns>
        public static string CreateExistsFilter(string path, bool exists = true)
        {
            return exists ? $"{path} ne null" : $"{path} eq null";
        }
    }
}
