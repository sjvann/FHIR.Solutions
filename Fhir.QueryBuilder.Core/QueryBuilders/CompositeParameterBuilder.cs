using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using ValidationResult = Fhir.QueryBuilder.Services.Interfaces.ValidationResult;

namespace Fhir.QueryBuilder.QueryBuilders
{
    /// <summary>
    /// Composite 參數建構器，整合到現有的參數建構器系統
    /// </summary>
    public class CompositeParameterBuilder : ISearchParameterBuilder
    {
        private readonly ILogger<CompositeParameterBuilder> _logger;

        public string ParameterType => "composite";

        public CompositeParameterBuilder(ILogger<CompositeParameterBuilder> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(string parameterType)
        {
            return string.Equals(parameterType, "composite", StringComparison.OrdinalIgnoreCase);
        }

        public string BuildParameter(SearchParameterContext context)
        {
            try
            {
                var parameterName = context.ParameterName;
                
                // 處理修飾符
                if (!string.IsNullOrEmpty(context.Modifier))
                {
                    parameterName = $"{parameterName}:{context.Modifier}";
                }

                // 建構 Composite 值
                var compositeValue = BuildCompositeValue(context);
                
                var result = $"{parameterName}={Uri.EscapeDataString(compositeValue)}";
                
                _logger.LogDebug("Built composite parameter: {Parameter}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building composite parameter: {ParameterName}", context.ParameterName);
                throw;
            }
        }

        public ValidationResult ValidateParameter(SearchParameterContext context)
        {
            var result = new ValidationResult { IsValid = true };

            try
            {
                // 驗證參數名稱
                if (string.IsNullOrWhiteSpace(context.ParameterName))
                {
                    result.AddError("Parameter name cannot be empty");
                    return result;
                }

                // 驗證組件
                if (!context.Values.ContainsKey("components"))
                {
                    result.AddError("Composite parameter must have components");
                    return result;
                }

                var components = GetComponents(context);
                if (components.Length < 2)
                {
                    result.AddError("Composite parameter must have at least 2 components");
                    return result;
                }

                // 驗證每個組件
                foreach (var component in components)
                {
                    if (string.IsNullOrWhiteSpace(component))
                    {
                        result.AddWarning("Empty component found in composite parameter");
                    }
                }

                _logger.LogDebug("Validated composite parameter: {ParameterName}, Valid: {IsValid}",
                    context.ParameterName, result.IsValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating composite parameter: {ParameterName}", context.ParameterName);
                result.AddError($"Validation error: {ex.Message}");
            }

            return result;
        }

        private string BuildCompositeValue(SearchParameterContext context)
        {
            var components = GetComponents(context);
            
            // 使用 $ 分隔符連接組件
            return string.Join("$", components.Select(c => c.Trim()));
        }

        private string[] GetComponents(SearchParameterContext context)
        {
            if (context.Values.TryGetValue("components", out var componentsObj))
            {
                return componentsObj switch
                {
                    string[] stringArray => stringArray,
                    string singleString => singleString.Split('$', StringSplitOptions.RemoveEmptyEntries),
                    IEnumerable<string> enumerable => enumerable.ToArray(),
                    _ => new[] { componentsObj.ToString() ?? string.Empty }
                };
            }

            // 嘗試從其他值建構組件
            var componentList = new List<string>();
            
            // 檢查是否有編號的組件 (component1, component2, etc.)
            for (int i = 1; i <= 10; i++) // 最多支援 10 個組件
            {
                var key = $"component{i}";
                if (context.Values.TryGetValue(key, out var componentValue))
                {
                    componentList.Add(componentValue.ToString() ?? string.Empty);
                }
                else
                {
                    break; // 沒有更多組件
                }
            }

            if (componentList.Count > 0)
            {
                return componentList.ToArray();
            }

            // 如果沒有找到組件，嘗試從所有值建構
            return context.Values.Values
                .Where(v => v != null)
                .Select(v => v.ToString() ?? string.Empty)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
        }
    }

    /// <summary>
    /// Composite 參數建構器的擴展方法
    /// </summary>
    public static class CompositeParameterExtensions
    {
        /// <summary>
        /// 建立 Token + Quantity 組合（常用於觀察值）
        /// </summary>
        /// <param name="system">代碼系統</param>
        /// <param name="code">代碼</param>
        /// <param name="value">數值</param>
        /// <param name="unit">單位</param>
        /// <param name="unitSystem">單位系統</param>
        /// <returns>組件陣列</returns>
        public static string[] CreateTokenQuantityComponents(
            string system, 
            string code, 
            decimal value, 
            string? unit = null, 
            string? unitSystem = null)
        {
            var tokenComponent = string.IsNullOrEmpty(system) ? code : $"{system}|{code}";
            
            var quantityComponent = value.ToString();
            if (!string.IsNullOrEmpty(unit))
            {
                if (!string.IsNullOrEmpty(unitSystem))
                {
                    quantityComponent = $"{value}|{unitSystem}|{unit}";
                }
                else
                {
                    quantityComponent = $"{value}||{unit}";
                }
            }

            return new[] { tokenComponent, quantityComponent };
        }

        /// <summary>
        /// 建立 Token + Date 組合
        /// </summary>
        /// <param name="system">代碼系統</param>
        /// <param name="code">代碼</param>
        /// <param name="date">日期</param>
        /// <returns>組件陣列</returns>
        public static string[] CreateTokenDateComponents(string system, string code, DateTime date)
        {
            var tokenComponent = string.IsNullOrEmpty(system) ? code : $"{system}|{code}";
            var dateComponent = date.ToString("yyyy-MM-dd");
            
            return new[] { tokenComponent, dateComponent };
        }

        /// <summary>
        /// 建立 String + String 組合
        /// </summary>
        /// <param name="component1">第一個組件</param>
        /// <param name="component2">第二個組件</param>
        /// <param name="additionalComponents">額外組件</param>
        /// <returns>組件陣列</returns>
        public static string[] CreateStringComponents(string component1, string component2, params string[] additionalComponents)
        {
            var components = new List<string> { component1, component2 };
            if (additionalComponents.Length > 0)
            {
                components.AddRange(additionalComponents);
            }
            return components.ToArray();
        }

        /// <summary>
        /// 建立 Reference + Token 組合
        /// </summary>
        /// <param name="reference">參考</param>
        /// <param name="system">代碼系統</param>
        /// <param name="code">代碼</param>
        /// <returns>組件陣列</returns>
        public static string[] CreateReferenceTokenComponents(string reference, string system, string code)
        {
            var tokenComponent = string.IsNullOrEmpty(system) ? code : $"{system}|{code}";
            return new[] { reference, tokenComponent };
        }
    }
}
