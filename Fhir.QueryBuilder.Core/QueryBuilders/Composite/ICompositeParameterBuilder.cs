using Fhir.QueryBuilder.Common;
using Fhir.QueryBuilder.QueryBuilders.Interfaces;

namespace Fhir.QueryBuilder.QueryBuilders.Composite
{
    /// <summary>Composite 定義（驗證用）。</summary>
    public class CompositeSearchParameter
    {
        public List<CompositeDefinitionComponent> Components { get; set; } = new();
    }

    public class CompositeDefinitionComponent
    {
        public SearchParameterType Type { get; set; }
    }

    /// <summary>
    /// Composite 參數建構器介面
    /// </summary>
    public interface ICompositeParameterBuilder
    {
        /// <summary>
        /// 新增組件
        /// </summary>
        /// <param name="value">組件值</param>
        /// <param name="type">組件類型</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddComponent(string value, SearchParameterType type);

        /// <summary>
        /// 新增 Token 組件
        /// </summary>
        /// <param name="system">系統</param>
        /// <param name="code">代碼</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddTokenComponent(string system, string code);

        /// <summary>
        /// 新增 Token 組件（僅代碼）
        /// </summary>
        /// <param name="code">代碼</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddTokenComponent(string code);

        /// <summary>
        /// 新增 String 組件
        /// </summary>
        /// <param name="value">字串值</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddStringComponent(string value);

        /// <summary>
        /// 新增 Number 組件
        /// </summary>
        /// <param name="value">數值</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddNumberComponent(decimal value);

        /// <summary>
        /// 新增 Date 組件
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddDateComponent(DateTime date);

        /// <summary>
        /// 新增 Date 組件（字串格式）
        /// </summary>
        /// <param name="dateString">日期字串</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddDateComponent(string dateString);

        /// <summary>
        /// 新增 Quantity 組件
        /// </summary>
        /// <param name="value">數值</param>
        /// <param name="unit">單位</param>
        /// <param name="system">單位系統</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddQuantityComponent(decimal value, string? unit = null, string? system = null);

        /// <summary>
        /// 新增 Reference 組件
        /// </summary>
        /// <param name="reference">參考</param>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder AddReferenceComponent(string reference);

        /// <summary>
        /// 建構 Composite 參數值
        /// </summary>
        /// <returns>Composite 參數值</returns>
        string Build();

        /// <summary>
        /// 驗證組件
        /// </summary>
        /// <param name="compositeDefinition">Composite 參數定義</param>
        /// <returns>驗證結果</returns>
        CompositeValidationResult Validate(CompositeSearchParameter compositeDefinition);

        /// <summary>
        /// 重設建構器
        /// </summary>
        /// <returns>建構器實例</returns>
        ICompositeParameterBuilder Reset();

        /// <summary>
        /// 取得組件數量
        /// </summary>
        int ComponentCount { get; }

        /// <summary>
        /// 取得組件列表
        /// </summary>
        IReadOnlyList<CompositeComponent> Components { get; }
    }

    /// <summary>
    /// Composite 驗證結果
    /// </summary>
    public class CompositeValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// 警告訊息
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// 建議
        /// </summary>
        public List<string> Suggestions { get; set; } = new();
    }

    /// <summary>
    /// Composite 組件
    /// </summary>
    public class CompositeComponent
    {
        /// <summary>
        /// 組件值
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 組件類型
        /// </summary>
        public SearchParameterType Type { get; set; }

        /// <summary>
        /// 原始值（編碼前）
        /// </summary>
        public string RawValue { get; set; } = string.Empty;

        /// <summary>
        /// 組件描述
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Composite 參數建構器實作
    /// </summary>
    public class CompositeParameterBuilder : ICompositeParameterBuilder
    {
        private readonly List<CompositeComponent> _components = new();

        public int ComponentCount => _components.Count;

        public IReadOnlyList<CompositeComponent> Components => _components.AsReadOnly();

        public ICompositeParameterBuilder AddComponent(string value, SearchParameterType type)
        {
            var encodedValue = EncodeComponentValue(value, type);
            _components.Add(new CompositeComponent
            {
                Value = encodedValue,
                Type = type,
                RawValue = value
            });
            return this;
        }

        public ICompositeParameterBuilder AddTokenComponent(string system, string code)
        {
            var tokenValue = $"{system}|{code}";
            return AddComponent(tokenValue, SearchParameterType.Token);
        }

        public ICompositeParameterBuilder AddTokenComponent(string code)
        {
            return AddComponent(code, SearchParameterType.Token);
        }

        public ICompositeParameterBuilder AddStringComponent(string value)
        {
            return AddComponent(value, SearchParameterType.String);
        }

        public ICompositeParameterBuilder AddNumberComponent(decimal value)
        {
            return AddComponent(value.ToString(), SearchParameterType.Number);
        }

        public ICompositeParameterBuilder AddDateComponent(DateTime date)
        {
            var dateString = date.ToString("yyyy-MM-dd");
            return AddComponent(dateString, SearchParameterType.Date);
        }

        public ICompositeParameterBuilder AddDateComponent(string dateString)
        {
            return AddComponent(dateString, SearchParameterType.Date);
        }

        public ICompositeParameterBuilder AddQuantityComponent(decimal value, string? unit = null, string? system = null)
        {
            var quantityValue = value.ToString();
            if (!string.IsNullOrEmpty(unit))
            {
                if (!string.IsNullOrEmpty(system))
                {
                    quantityValue = $"{value}|{system}|{unit}";
                }
                else
                {
                    quantityValue = $"{value}||{unit}";
                }
            }
            return AddComponent(quantityValue, SearchParameterType.Quantity);
        }

        public ICompositeParameterBuilder AddReferenceComponent(string reference)
        {
            return AddComponent(reference, SearchParameterType.Reference);
        }

        public string Build()
        {
            if (!_components.Any())
            {
                throw new InvalidOperationException("At least one component must be added before building");
            }

            return string.Join("$", _components.Select(c => c.Value));
        }

        public CompositeValidationResult Validate(CompositeSearchParameter compositeDefinition)
        {
            var result = new CompositeValidationResult { IsValid = true };

            // 檢查組件數量
            if (_components.Count != compositeDefinition.Components.Count)
            {
                result.Errors.Add($"Expected {compositeDefinition.Components.Count} components, but got {_components.Count}");
                result.IsValid = false;
            }

            // 檢查每個組件的類型
            for (int i = 0; i < Math.Min(_components.Count, compositeDefinition.Components.Count); i++)
            {
                var component = _components[i];
                var definition = compositeDefinition.Components[i];

                if (component.Type != definition.Type)
                {
                    result.Errors.Add($"Component {i + 1}: Expected type {definition.Type}, but got {component.Type}");
                    result.IsValid = false;
                }

                // 驗證組件值
                var valueValidation = ValidateComponentValue(component, definition);
                if (!valueValidation.IsValid)
                {
                    result.Errors.AddRange(valueValidation.Errors.Select(e => $"Component {i + 1}: {e}"));
                    result.IsValid = false;
                }
                
                result.Warnings.AddRange(valueValidation.Warnings.Select(w => $"Component {i + 1}: {w}"));
                result.Suggestions.AddRange(valueValidation.Suggestions.Select(s => $"Component {i + 1}: {s}"));
            }

            return result;
        }

        public ICompositeParameterBuilder Reset()
        {
            _components.Clear();
            return this;
        }

        private string EncodeComponentValue(string value, SearchParameterType type)
        {
            // 根據類型進行適當的編碼
            return type switch
            {
                SearchParameterType.String => Uri.EscapeDataString(value),
                SearchParameterType.Token => value, // Token 值通常已經是正確格式
                SearchParameterType.Number => value,
                SearchParameterType.Date => value,
                SearchParameterType.Quantity => value,
                SearchParameterType.Reference => Uri.EscapeDataString(value),
                SearchParameterType.Uri => Uri.EscapeDataString(value),
                _ => Uri.EscapeDataString(value)
            };
        }

        private CompositeValidationResult ValidateComponentValue(CompositeComponent component, CompositeDefinitionComponent definition)
        {
            var result = new CompositeValidationResult { IsValid = true };

            switch (component.Type)
            {
                case SearchParameterType.Token:
                    ValidateTokenComponent(component, result);
                    break;
                case SearchParameterType.Number:
                    ValidateNumberComponent(component, result);
                    break;
                case SearchParameterType.Date:
                    ValidateDateComponent(component, result);
                    break;
                case SearchParameterType.Quantity:
                    ValidateQuantityComponent(component, result);
                    break;
                case SearchParameterType.String:
                    ValidateStringComponent(component, result);
                    break;
                case SearchParameterType.Reference:
                    ValidateReferenceComponent(component, result);
                    break;
            }

            return result;
        }

        private void ValidateTokenComponent(CompositeComponent component, CompositeValidationResult result)
        {
            var value = component.RawValue;
            
            // Token 可以是 "code" 或 "system|code" 格式
            if (value.Contains('|'))
            {
                var parts = value.Split('|');
                if (parts.Length != 2)
                {
                    result.Errors.Add("Token format should be 'system|code'");
                    result.IsValid = false;
                }
                else if (string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                {
                    result.Errors.Add("Both system and code must be non-empty in 'system|code' format");
                    result.IsValid = false;
                }
            }
            else if (string.IsNullOrEmpty(value))
            {
                result.Errors.Add("Token code cannot be empty");
                result.IsValid = false;
            }
        }

        private void ValidateNumberComponent(CompositeComponent component, CompositeValidationResult result)
        {
            if (!decimal.TryParse(component.RawValue, out _))
            {
                result.Errors.Add($"'{component.RawValue}' is not a valid number");
                result.IsValid = false;
            }
        }

        private void ValidateDateComponent(CompositeComponent component, CompositeValidationResult result)
        {
            var value = component.RawValue;
            
            // 支援多種日期格式
            var validFormats = new[]
            {
                "yyyy",
                "yyyy-MM",
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss.fffZ"
            };

            var isValid = validFormats.Any(format => 
                DateTime.TryParseExact(value, format, null, System.Globalization.DateTimeStyles.None, out _));

            if (!isValid)
            {
                result.Errors.Add($"'{value}' is not a valid date format");
                result.IsValid = false;
                result.Suggestions.Add("Use formats like: yyyy, yyyy-MM, yyyy-MM-dd, or yyyy-MM-ddTHH:mm:ss");
            }
        }

        private void ValidateQuantityComponent(CompositeComponent component, CompositeValidationResult result)
        {
            var value = component.RawValue;
            
            if (value.Contains('|'))
            {
                var parts = value.Split('|');
                if (parts.Length != 3)
                {
                    result.Errors.Add("Quantity format should be 'value|system|unit' or just 'value'");
                    result.IsValid = false;
                }
                else if (!decimal.TryParse(parts[0], out _))
                {
                    result.Errors.Add($"'{parts[0]}' is not a valid quantity value");
                    result.IsValid = false;
                }
            }
            else if (!decimal.TryParse(value, out _))
            {
                result.Errors.Add($"'{value}' is not a valid quantity value");
                result.IsValid = false;
            }
        }

        private void ValidateStringComponent(CompositeComponent component, CompositeValidationResult result)
        {
            if (string.IsNullOrEmpty(component.RawValue))
            {
                result.Warnings.Add("String component is empty");
            }
        }

        private void ValidateReferenceComponent(CompositeComponent component, CompositeValidationResult result)
        {
            var value = component.RawValue;
            
            if (string.IsNullOrEmpty(value))
            {
                result.Errors.Add("Reference cannot be empty");
                result.IsValid = false;
            }
            else if (!IsValidReference(value))
            {
                result.Warnings.Add($"'{value}' may not be a valid reference format");
                result.Suggestions.Add("Use formats like: 'ResourceType/id', 'id', or full URL");
            }
        }

        private bool IsValidReference(string reference)
        {
            // 簡化的參考驗證
            if (Uri.TryCreate(reference, UriKind.Absolute, out _))
            {
                return true; // 完整 URL
            }

            if (reference.Contains('/'))
            {
                var parts = reference.Split('/');
                return parts.Length == 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]);
            }

            return !string.IsNullOrEmpty(reference); // 簡單 ID
        }
    }
}
