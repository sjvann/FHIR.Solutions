using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public abstract class BaseSearchParameterBuilder : ISearchParameterBuilder
    {
        protected readonly ILogger _logger;

        protected BaseSearchParameterBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public abstract string ParameterType { get; }

        public virtual bool CanHandle(string parameterType)
        {
            return string.Equals(ParameterType, parameterType, StringComparison.OrdinalIgnoreCase);
        }

        public abstract string BuildParameter(SearchParameterContext context);

        public virtual ValidationResult ValidateParameter(SearchParameterContext context)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(context.ParameterName))
            {
                result.AddError("Parameter name cannot be empty");
            }

            if (!CanHandle(context.ParameterType))
            {
                result.AddError($"Cannot handle parameter type: {context.ParameterType}");
            }

            return result;
        }

        protected virtual string BuildParameterString(string parameterName, string value, string? modifier = null)
        {
            var sb = new StringBuilder();
            sb.Append(parameterName);

            if (!string.IsNullOrEmpty(modifier))
            {
                sb.Append($":{modifier}");
            }

            sb.Append($"={value}");
            return sb.ToString();
        }

        protected virtual string GetValueFromContext(SearchParameterContext context, string key, string defaultValue = "")
        {
            if (context.Values.TryGetValue(key, out var value))
            {
                return value?.ToString() ?? defaultValue;
            }
            return defaultValue;
        }

        protected virtual bool GetBooleanFromContext(SearchParameterContext context, string key, bool defaultValue = false)
        {
            if (context.Values.TryGetValue(key, out var value))
            {
                if (value is bool boolValue)
                    return boolValue;
                
                if (bool.TryParse(value?.ToString(), out var parsedValue))
                    return parsedValue;
            }
            return defaultValue;
        }

        protected virtual void LogParameterBuilding(SearchParameterContext context, string result)
        {
            _logger.LogDebug("Built {ParameterType} parameter: {ParameterName} -> {Result}", 
                context.ParameterType, context.ParameterName, result);
        }
    }
}
