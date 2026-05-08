using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Fhir.QueryBuilder.QueryBuilders.FluentApi
{
    public class FhirQueryBuilder : IFhirQueryBuilder
    {
        private readonly ISearchParameterFactory _parameterFactory;
        private readonly IValidationService _validationService;
        private readonly ILogger<FhirQueryBuilder> _logger;
        
        private string _resourceType = string.Empty;
        private readonly List<string> _parameters = new();
        private readonly List<string> _includes = new();
        private readonly List<string> _revIncludes = new();
        private readonly List<string> _modifyingParameters = new();
        private readonly List<string> _validationErrors = new();

        public FhirQueryBuilder(
            ISearchParameterFactory parameterFactory,
            IValidationService validationService,
            ILogger<FhirQueryBuilder> logger)
        {
            _parameterFactory = parameterFactory;
            _validationService = validationService;
            _logger = logger;
        }

        public IFhirQueryBuilder ForResource(string resourceType)
        {
            if (string.IsNullOrWhiteSpace(resourceType))
            {
                _validationErrors.Add("Resource type cannot be empty");
                return this;
            }

            _resourceType = resourceType;
            _logger.LogDebug("Set resource type to: {ResourceType}", resourceType);
            return this;
        }

        public IFhirQueryBuilder ForResource<T>() where T : class
        {
            var resourceType = typeof(T).Name;
            return ForResource(resourceType);
        }

        public IFhirQueryBuilder WhereString(string parameterName, string value, SearchModifier modifier = SearchModifier.None)
        {
            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "string",
                Values = new Dictionary<string, object> { { "value", value } },
                Modifier = GetModifierString(modifier)
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder WhereDate(string parameterName, DateTime value, SearchPrefix prefix = SearchPrefix.None)
        {
            return WhereDate(parameterName, value.ToString("yyyy-MM-dd"), prefix);
        }

        public IFhirQueryBuilder WhereDate(string parameterName, string value, SearchPrefix prefix = SearchPrefix.None)
        {
            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "date",
                Values = new Dictionary<string, object> { { "value", value } },
                Prefix = GetPrefixString(prefix)
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder WhereNumber(string parameterName, decimal value, SearchPrefix prefix = SearchPrefix.None)
        {
            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "number",
                Values = new Dictionary<string, object> { { "value", value.ToString() } },
                Prefix = GetPrefixString(prefix)
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder WhereToken(string parameterName, string code, string? system = null)
        {
            var values = new Dictionary<string, object> { { "code", code } };
            if (!string.IsNullOrEmpty(system))
            {
                values.Add("system", system);
            }

            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "token",
                Values = values
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder WhereReference(string parameterName, string id)
        {
            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "reference",
                Values = new Dictionary<string, object> { { "id", id } }
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder WhereReference(string parameterName, string resourceType, string id)
        {
            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "reference",
                Values = new Dictionary<string, object> 
                { 
                    { "resourceType", resourceType },
                    { "id", id }
                }
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder WhereReferenceUrl(string parameterName, string url)
        {
            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "reference",
                Values = new Dictionary<string, object> { { "url", url } }
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder WhereQuantity(string parameterName, decimal number, string? code = null, string? system = null, SearchPrefix prefix = SearchPrefix.None)
        {
            var values = new Dictionary<string, object> { { "number", number.ToString() } };
            if (!string.IsNullOrEmpty(code))
            {
                values.Add("code", code);
            }
            if (!string.IsNullOrEmpty(system))
            {
                values.Add("system", system);
            }

            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "quantity",
                Values = values,
                Prefix = GetPrefixString(prefix)
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder WhereUri(string parameterName, string uri)
        {
            var context = new SearchParameterContext
            {
                ParameterName = parameterName,
                ParameterType = "uri",
                Values = new Dictionary<string, object> { { "value", uri } }
            };

            return AddParameter(context);
        }

        public IFhirQueryBuilder Where(string parameterName, string value, SearchModifier modifier = SearchModifier.None)
        {
            // Generic method - assumes string type
            return WhereString(parameterName, value, modifier);
        }

        public IFhirQueryBuilder Include(string includePath)
        {
            if (!string.IsNullOrWhiteSpace(includePath))
            {
                _includes.Add(includePath);
                _logger.LogDebug("Added include: {IncludePath}", includePath);
            }
            return this;
        }

        public IFhirQueryBuilder RevInclude(string revIncludePath)
        {
            if (!string.IsNullOrWhiteSpace(revIncludePath))
            {
                _revIncludes.Add(revIncludePath);
                _logger.LogDebug("Added revinclude: {RevIncludePath}", revIncludePath);
            }
            return this;
        }

        public IFhirQueryBuilder Chain(string chainPath, string value)
        {
            if (!string.IsNullOrWhiteSpace(chainPath) && !string.IsNullOrWhiteSpace(value))
            {
                var parameter = $"{chainPath}={Uri.EscapeDataString(value)}";
                _parameters.Add(parameter);
                _logger.LogDebug("Added chain parameter: {ChainPath}={Value}", chainPath, value);
            }
            return this;
        }

        public IFhirQueryBuilder ReverseChain(string resourceType, string searchParam, string value)
        {
            if (!string.IsNullOrWhiteSpace(resourceType) && !string.IsNullOrWhiteSpace(searchParam) && !string.IsNullOrWhiteSpace(value))
            {
                var hasParam = $"_has:{resourceType}:{searchParam}:{value}";
                _modifyingParameters.Add($"_has={Uri.EscapeDataString(hasParam)}");
                _logger.LogDebug("Added reverse chain parameter: {HasParam}", hasParam);
            }
            return this;
        }

        public IFhirQueryBuilder WhereComposite(string parameterName, params string[] components)
        {
            if (!string.IsNullOrWhiteSpace(parameterName) && components.Length >= 2)
            {
                var compositeValue = string.Join("$", components.Select(Uri.EscapeDataString));
                var parameter = $"{parameterName}={compositeValue}";
                _parameters.Add(parameter);
                _logger.LogDebug("Added composite parameter: {ParameterName}={CompositeValue}", parameterName, compositeValue);
            }
            return this;
        }

        public IFhirQueryBuilder Filter(string filterExpression)
        {
            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                _modifyingParameters.Add($"_filter={Uri.EscapeDataString(filterExpression)}");
                _logger.LogDebug("Added filter: {FilterExpression}", filterExpression);
            }
            return this;
        }

        public IFhirQueryBuilder Count(int count)
        {
            if (count > 0)
            {
                _modifyingParameters.Add($"_count={count}");
                _logger.LogDebug("Set count to: {Count}", count);
            }
            return this;
        }

        public IFhirQueryBuilder Offset(int offset)
        {
            if (offset >= 0)
            {
                _modifyingParameters.Add($"_offset={offset}");
                _logger.LogDebug("Set offset to: {Offset}", offset);
            }
            return this;
        }

        public IFhirQueryBuilder MaxResults(int maxResults)
        {
            if (maxResults > 0)
            {
                _modifyingParameters.Add($"_maxresults={maxResults}");
                _logger.LogDebug("Set max results to: {MaxResults}", maxResults);
            }
            return this;
        }

        public IFhirQueryBuilder Summary(string summaryType = "true")
        {
            if (!string.IsNullOrWhiteSpace(summaryType))
            {
                _modifyingParameters.Add($"_summary={summaryType}");
                _logger.LogDebug("Set summary to: {SummaryType}", summaryType);
            }
            return this;
        }

        public IFhirQueryBuilder Elements(params string[] elements)
        {
            if (elements.Length > 0)
            {
                var elementsList = string.Join(",", elements);
                _modifyingParameters.Add($"_elements={elementsList}");
                _logger.LogDebug("Set elements to: {Elements}", elementsList);
            }
            return this;
        }

        public IFhirQueryBuilder Sort(params string[] sortFields)
        {
            if (sortFields.Length > 0)
            {
                var sortList = string.Join(",", sortFields);
                _modifyingParameters.Add($"_sort={sortList}");
                _logger.LogDebug("Set sort to: {Sort}", sortList);
            }
            return this;
        }

        public IFhirQueryBuilder Total(string totalMode = "accurate")
        {
            if (!string.IsNullOrWhiteSpace(totalMode))
            {
                _modifyingParameters.Add($"_total={totalMode}");
                _logger.LogDebug("Set total mode to: {TotalMode}", totalMode);
            }
            return this;
        }

        public IFhirQueryBuilder Contained(string containedMode = "true")
        {
            if (!string.IsNullOrWhiteSpace(containedMode))
            {
                _modifyingParameters.Add($"_contained={containedMode}");
                _logger.LogDebug("Set contained mode to: {ContainedMode}", containedMode);
            }
            return this;
        }

        public string BuildUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(_resourceType))
            {
                throw new InvalidOperationException("Resource type must be specified");
            }

            var url = baseUrl.TrimEnd('/') + "/" + _resourceType;
            var queryString = BuildQueryString();
            
            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + queryString;
            }

            _logger.LogDebug("Built URL: {Url}", url);
            return url;
        }

        public string BuildQueryString()
        {
            var allParameters = new List<string>();
            
            // Add search parameters
            allParameters.AddRange(_parameters);
            
            // Add includes
            if (_includes.Count > 0)
            {
                allParameters.Add($"_include={string.Join(",", _includes)}");
            }
            
            // Add rev-includes
            if (_revIncludes.Count > 0)
            {
                allParameters.Add($"_revinclude={string.Join(",", _revIncludes)}");
            }
            
            // Add modifying parameters
            allParameters.AddRange(_modifyingParameters);

            return string.Join("&", allParameters);
        }

        public bool IsValid()
        {
            _validationErrors.Clear();

            if (string.IsNullOrWhiteSpace(_resourceType))
            {
                _validationErrors.Add("Resource type must be specified");
            }

            // Additional validation can be added here
            return _validationErrors.Count == 0;
        }

        public IEnumerable<string> GetValidationErrors()
        {
            return _validationErrors.AsReadOnly();
        }

        private IFhirQueryBuilder AddParameter(SearchParameterContext context)
        {
            try
            {
                var builder = _parameterFactory.GetBuilder(context.ParameterType);
                if (builder == null)
                {
                    _validationErrors.Add($"No builder found for parameter type: {context.ParameterType}");
                    return this;
                }

                var validationResult = builder.ValidateParameter(context);
                if (!validationResult.IsValid)
                {
                    _validationErrors.AddRange(validationResult.Errors);
                    return this;
                }

                var parameter = builder.BuildParameter(context);
                _parameters.Add(parameter);
                
                _logger.LogDebug("Added parameter: {Parameter}", parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding parameter: {ParameterName}", context.ParameterName);
                _validationErrors.Add($"Error adding parameter {context.ParameterName}: {ex.Message}");
            }

            return this;
        }

        private static string? GetModifierString(SearchModifier modifier)
        {
            return modifier switch
            {
                SearchModifier.None => null,
                SearchModifier.Exact => "exact",
                SearchModifier.Contains => "contains",
                SearchModifier.Missing => "missing",
                SearchModifier.Text => "text",
                SearchModifier.Not => "not",
                SearchModifier.Above => "above",
                SearchModifier.Below => "below",
                SearchModifier.In => "in",
                SearchModifier.NotIn => "not-in",
                SearchModifier.Identifier => "identifier",
                _ => null
            };
        }

        private static string? GetPrefixString(SearchPrefix prefix)
        {
            return prefix switch
            {
                SearchPrefix.None => null,
                SearchPrefix.Equal => "eq",
                SearchPrefix.NotEqual => "ne",
                SearchPrefix.GreaterThan => "gt",
                SearchPrefix.LessThan => "lt",
                SearchPrefix.GreaterEqual => "ge",
                SearchPrefix.LessEqual => "le",
                SearchPrefix.StartsAfter => "sa",
                SearchPrefix.EndsBefore => "eb",
                SearchPrefix.Approximate => "ap",
                _ => null
            };
        }
    }
}
