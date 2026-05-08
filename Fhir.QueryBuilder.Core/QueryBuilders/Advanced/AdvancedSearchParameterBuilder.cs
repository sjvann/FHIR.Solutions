using Microsoft.Extensions.Logging;
using Fhir.QueryBuilder.Common;
using System.Text;

namespace Fhir.QueryBuilder.QueryBuilders.Advanced
{
    /// <summary>
    /// 進階搜尋參數建構器實作
    /// </summary>
    public class AdvancedSearchParameterBuilder : IAdvancedSearchParameterBuilder
    {
        private readonly ISearchParameterRegistry _searchRegistry;
        private readonly ILogger<AdvancedSearchParameterBuilder> _logger;
        
        private readonly List<SearchParameter> _parameters = new();
        private readonly List<string> _includes = new();
        private readonly List<string> _revIncludes = new();
        private readonly List<SortParameter> _sorts = new();
        private string? _filter;
        private int? _count;
        private int? _offset;
        private TotalMode? _totalMode;
        private SummaryMode? _summaryMode;
        private string[]? _elements;

        public AdvancedSearchParameterBuilder(
            ISearchParameterRegistry searchRegistry,
            ILogger<AdvancedSearchParameterBuilder> logger)
        {
            _searchRegistry = searchRegistry;
            _logger = logger;
        }

        public IAdvancedSearchParameterBuilder AddParameter(string name, string value)
        {
            _parameters.Add(new SearchParameter
            {
                Name = name,
                Value = value
            });
            return this;
        }

        public IAdvancedSearchParameterBuilder AddParameter(string name, SearchModifier modifier, string value)
        {
            _parameters.Add(new SearchParameter
            {
                Name = name,
                Modifier = modifier,
                Value = value
            });
            return this;
        }

        public IAdvancedSearchParameterBuilder AddParameter(string name, SearchPrefix prefix, string value)
        {
            _parameters.Add(new SearchParameter
            {
                Name = name,
                Prefix = prefix,
                Value = value
            });
            return this;
        }

        public IAdvancedSearchParameterBuilder AddCompositeParameter(string name, params string[] components)
        {
            if (components.Length < 2)
            {
                throw new ArgumentException("Composite parameters must have at least 2 components", nameof(components));
            }

            // Raw components joined by '$'; Build() applies Uri.EscapeDataString once on the full value.
            var compositeValue = string.Join("$", components);

            _parameters.Add(new SearchParameter
            {
                Name = name,
                Value = compositeValue,
                IsComposite = true
            });
            return this;
        }

        public IAdvancedSearchParameterBuilder AddChainParameter(string chain, string value)
        {
            _parameters.Add(new SearchParameter
            {
                Name = chain,
                Value = value,
                IsChained = true
            });
            return this;
        }

        public IAdvancedSearchParameterBuilder AddReverseChainParameter(string resourceType, string searchParam, string value)
        {
            var hasParam = $"_has:{resourceType}:{searchParam}:{value}";
            _parameters.Add(new SearchParameter
            {
                Name = "_has",
                Value = hasParam,
                IsReverseChained = true
            });
            return this;
        }

        public IAdvancedSearchParameterBuilder AddInclude(string include)
        {
            _includes.Add(include);
            return this;
        }

        public IAdvancedSearchParameterBuilder AddRevInclude(string revinclude)
        {
            _revIncludes.Add(revinclude);
            return this;
        }

        public IAdvancedSearchParameterBuilder SetSort(string parameter, bool descending = false)
        {
            _sorts.Add(new SortParameter
            {
                Parameter = parameter,
                Descending = descending
            });
            return this;
        }

        public IAdvancedSearchParameterBuilder SetPaging(int count, int offset = 0)
        {
            _count = count;
            _offset = offset;
            return this;
        }

        public IAdvancedSearchParameterBuilder SetTotal(TotalMode totalMode)
        {
            _totalMode = totalMode;
            return this;
        }

        public IAdvancedSearchParameterBuilder SetSummary(SummaryMode summaryMode)
        {
            _summaryMode = summaryMode;
            return this;
        }

        public IAdvancedSearchParameterBuilder SetElements(params string[] elements)
        {
            _elements = elements;
            return this;
        }

        public IAdvancedSearchParameterBuilder AddFilter(string filter)
        {
            _filter = filter;
            return this;
        }

        public string Build()
        {
            var queryParams = new List<string>();

            // 基本搜尋參數
            foreach (var param in _parameters)
            {
                var paramName = BuildParameterName(param);
                var paramValue = Uri.EscapeDataString(param.Value);
                queryParams.Add($"{paramName}={paramValue}");
            }

            // Include 參數
            foreach (var include in _includes)
            {
                queryParams.Add($"_include={Uri.EscapeDataString(include)}");
            }

            // RevInclude 參數
            foreach (var revInclude in _revIncludes)
            {
                queryParams.Add($"_revinclude={Uri.EscapeDataString(revInclude)}");
            }

            // 排序參數
            if (_sorts.Any())
            {
                var sortValues = _sorts.Select(s => s.Descending ? $"-{s.Parameter}" : s.Parameter);
                queryParams.Add($"_sort={Uri.EscapeDataString(string.Join(",", sortValues))}");
            }

            // 分頁參數
            if (_count.HasValue)
            {
                queryParams.Add($"_count={_count.Value}");
            }

            if (_offset.HasValue && _offset.Value > 0)
            {
                queryParams.Add($"_offset={_offset.Value}");
            }

            // 總數控制
            if (_totalMode.HasValue)
            {
                var totalValue = _totalMode.Value switch
                {
                    TotalMode.None => "none",
                    TotalMode.Estimate => "estimate",
                    TotalMode.Accurate => "accurate",
                    _ => "none"
                };
                queryParams.Add($"_total={totalValue}");
            }

            // 摘要模式
            if (_summaryMode.HasValue)
            {
                var summaryValue = _summaryMode.Value switch
                {
                    SummaryMode.False => "false",
                    SummaryMode.True => "true",
                    SummaryMode.Text => "text",
                    SummaryMode.Data => "data",
                    SummaryMode.Count => "count",
                    _ => "false"
                };
                queryParams.Add($"_summary={summaryValue}");
            }

            // 元素選擇
            if (_elements?.Any() == true)
            {
                queryParams.Add($"_elements={Uri.EscapeDataString(string.Join(",", _elements))}");
            }

            // Filter 參數
            if (!string.IsNullOrEmpty(_filter))
            {
                queryParams.Add($"_filter={Uri.EscapeDataString(_filter)}");
            }

            return string.Join("&", queryParams);
        }

        public async Task<SearchParameterValidationResult> ValidateAsync(string resourceType, FhirVersion version)
        {
            var result = new SearchParameterValidationResult { IsValid = true };

            try
            {
                // 驗證基本搜尋參數
                foreach (var param in _parameters)
                {
                    await ValidateSearchParameterAsync(param, resourceType, version, result);
                }

                // 驗證 Include 參數
                foreach (var include in _includes)
                {
                    ValidateIncludeParameter(include, resourceType, result);
                }

                // 驗證排序參數
                foreach (var sort in _sorts)
                {
                    await ValidateSortParameterAsync(sort, resourceType, version, result);
                }

                // 驗證元素選擇
                if (_elements?.Any() == true)
                {
                    ValidateElementsParameter(_elements, resourceType, result);
                }

                result.IsValid = !result.Errors.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating search parameters");
                result.Errors.Add(new ValidationError
                {
                    ParameterName = "validation",
                    Message = $"Validation failed: {ex.Message}",
                    Type = ValidationErrorType.SyntaxError
                });
                result.IsValid = false;
            }

            return result;
        }

        public IAdvancedSearchParameterBuilder Reset()
        {
            _parameters.Clear();
            _includes.Clear();
            _revIncludes.Clear();
            _sorts.Clear();
            _filter = null;
            _count = null;
            _offset = null;
            _totalMode = null;
            _summaryMode = null;
            _elements = null;
            return this;
        }

        private string BuildParameterName(SearchParameter param)
        {
            var name = param.Name;

            // 處理修飾符
            if (param.Modifier.HasValue)
            {
                var modifierString = param.Modifier.Value switch
                {
                    SearchModifier.Missing => "missing",
                    SearchModifier.Exact => "exact",
                    SearchModifier.Contains => "contains",
                    SearchModifier.Text => "text",
                    SearchModifier.Not => "not",
                    SearchModifier.Above => "above",
                    SearchModifier.Below => "below",
                    SearchModifier.In => "in",
                    SearchModifier.NotIn => "not-in",
                    SearchModifier.OfType => "of-type",
                    SearchModifier.Identifier => "identifier",
                    SearchModifier.TextAdvanced => "text-advanced",
                    SearchModifier.CodeText => "code-text",
                    SearchModifier.Iterate => "iterate",
                    _ => throw new ArgumentException($"Unknown modifier: {param.Modifier}")
                };
                name = $"{name}:{modifierString}";
            }

            return name;
        }

        private string BuildParameterValue(SearchParameter param)
        {
            var value = param.Value;

            // 處理前綴
            if (param.Prefix.HasValue)
            {
                var prefixString = param.Prefix.Value switch
                {
                    SearchPrefix.Equal => "eq",
                    SearchPrefix.NotEqual => "ne",
                    SearchPrefix.GreaterThan => "gt",
                    SearchPrefix.LessThan => "lt",
                    SearchPrefix.GreaterThanOrEqual => "ge",
                    SearchPrefix.LessThanOrEqual => "le",
                    SearchPrefix.StartAfter => "sa",
                    SearchPrefix.EndBefore => "eb",
                    SearchPrefix.Approximate => "ap",
                    _ => throw new ArgumentException($"Unknown prefix: {param.Prefix}")
                };
                value = $"{prefixString}{value}";
            }

            return value;
        }

        private async Task ValidateSearchParameterAsync(
            SearchParameter param, 
            string resourceType, 
            FhirVersion version, 
            SearchParameterValidationResult result)
        {
            // 檢查參數是否存在
            var paramDef = await _searchRegistry.GetSearchParameterAsync(resourceType, param.Name, version);
            if (paramDef == null)
            {
                result.Errors.Add(new ValidationError
                {
                    ParameterName = param.Name,
                    Message = $"Unknown search parameter '{param.Name}' for resource type '{resourceType}'",
                    Type = ValidationErrorType.UnknownParameter
                });
                return;
            }

            // 檢查修飾符是否支援
            if (param.Modifier.HasValue)
            {
                var supportedModifiers = await _searchRegistry.GetParameterModifiersAsync(resourceType, param.Name, version);
                var modifierString = GetModifierString(param.Modifier.Value);
                
                if (!supportedModifiers.Contains(modifierString))
                {
                    result.Errors.Add(new ValidationError
                    {
                        ParameterName = param.Name,
                        Message = $"Modifier '{modifierString}' is not supported for parameter '{param.Name}'",
                        Type = ValidationErrorType.UnsupportedModifier
                    });
                }
            }

            // 檢查前綴是否適用於參數類型
            if (param.Prefix.HasValue)
            {
                var supportsPrefixes = paramDef.Type switch
                {
                    SearchParameterType.Number => true,
                    SearchParameterType.Date => true,
                    SearchParameterType.Quantity => true,
                    _ => false
                };

                if (!supportsPrefixes)
                {
                    result.Errors.Add(new ValidationError
                    {
                        ParameterName = param.Name,
                        Message = $"Prefixes are not supported for parameter type '{paramDef.Type}'",
                        Type = ValidationErrorType.UnsupportedPrefix
                    });
                }
            }
        }

        private void ValidateIncludeParameter(string include, string resourceType, SearchParameterValidationResult result)
        {
            // 驗證 Include 參數格式：ResourceType:searchParam(:targetType)
            var parts = include.Split(':');
            if (parts.Length < 2 || parts.Length > 3)
            {
                result.Errors.Add(new ValidationError
                {
                    ParameterName = "_include",
                    Message = $"Invalid include format: '{include}'. Expected format: 'ResourceType:searchParam(:targetType)'",
                    Type = ValidationErrorType.SyntaxError
                });
            }
        }

        private async Task ValidateSortParameterAsync(
            SortParameter sort, 
            string resourceType, 
            FhirVersion version, 
            SearchParameterValidationResult result)
        {
            var paramDef = await _searchRegistry.GetSearchParameterAsync(resourceType, sort.Parameter, version);
            if (paramDef == null)
            {
                result.Errors.Add(new ValidationError
                {
                    ParameterName = sort.Parameter,
                    Message = $"Unknown sort parameter '{sort.Parameter}' for resource type '{resourceType}'",
                    Type = ValidationErrorType.UnknownParameter
                });
            }
        }

        private void ValidateElementsParameter(string[] elements, string resourceType, SearchParameterValidationResult result)
        {
            // 基本驗證元素路徑格式
            foreach (var element in elements)
            {
                if (string.IsNullOrWhiteSpace(element))
                {
                    result.Errors.Add(new ValidationError
                    {
                        ParameterName = "_elements",
                        Message = "Element path cannot be empty",
                        Type = ValidationErrorType.InvalidValue
                    });
                }
            }
        }

        private string GetModifierString(SearchModifier modifier)
        {
            return modifier switch
            {
                SearchModifier.Missing => "missing",
                SearchModifier.Exact => "exact",
                SearchModifier.Contains => "contains",
                SearchModifier.Text => "text",
                SearchModifier.Not => "not",
                SearchModifier.Above => "above",
                SearchModifier.Below => "below",
                SearchModifier.In => "in",
                SearchModifier.NotIn => "not-in",
                SearchModifier.OfType => "of-type",
                SearchModifier.Identifier => "identifier",
                SearchModifier.TextAdvanced => "text-advanced",
                SearchModifier.CodeText => "code-text",
                SearchModifier.Iterate => "iterate",
                _ => throw new ArgumentException($"Unknown modifier: {modifier}")
            };
        }

        private class SearchParameter
        {
            public string Name { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public SearchModifier? Modifier { get; set; }
            public SearchPrefix? Prefix { get; set; }
            public bool IsComposite { get; set; }
            public bool IsChained { get; set; }
            public bool IsReverseChained { get; set; }
        }

        private class SortParameter
        {
            public string Parameter { get; set; } = string.Empty;
            public bool Descending { get; set; }
        }
    }
}
