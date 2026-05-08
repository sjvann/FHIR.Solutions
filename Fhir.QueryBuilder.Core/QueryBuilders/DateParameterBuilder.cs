using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public class DateParameterBuilder : BaseSearchParameterBuilder
    {
        private static readonly Regex FhirDateRegex = new(@"^\d{4}(-\d{2}(-\d{2})?)?$", RegexOptions.Compiled);
        private static readonly Regex FhirDateTimeRegex = new(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{3})?(Z|[+-]\d{2}:\d{2})$", RegexOptions.Compiled);

        public DateParameterBuilder(ILogger<DateParameterBuilder> logger) : base(logger)
        {
        }

        public override string ParameterType => "date";

        public override string BuildParameter(SearchParameterContext context)
        {
            var value = GetValueFromContext(context, "value");
            var prefix = GetValueFromContext(context, "prefix", context.Prefix ?? "");
            
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Date parameter value cannot be empty");
            }

            var parameterValue = string.IsNullOrEmpty(prefix) ? value : $"{prefix}{value}";
            var result = BuildParameterString(context.ParameterName, parameterValue, context.Modifier);
            LogParameterBuilding(context, result);
            return result;
        }

        public override ValidationResult ValidateParameter(SearchParameterContext context)
        {
            var result = base.ValidateParameter(context);
            
            var value = GetValueFromContext(context, "value");
            if (string.IsNullOrEmpty(value))
            {
                result.AddError("Date parameter value cannot be empty");
                return result;
            }

            // Validate date format
            if (!FhirDateRegex.IsMatch(value) && !FhirDateTimeRegex.IsMatch(value))
            {
                result.AddError("Invalid date format. Use YYYY, YYYY-MM, YYYY-MM-DD, or full datetime format");
            }

            // Validate prefix
            var prefix = GetValueFromContext(context, "prefix", context.Prefix ?? "");
            var supportedPrefixes = new[] { "eq", "ne", "gt", "lt", "ge", "le", "sa", "eb", "ap" };
            if (!string.IsNullOrEmpty(prefix) && !supportedPrefixes.Contains(prefix))
            {
                result.AddError($"Unsupported date prefix: {prefix}");
            }

            // Validate supported modifiers for date parameters
            var supportedModifiers = new[] { "missing" };
            if (!string.IsNullOrEmpty(context.Modifier) && !supportedModifiers.Contains(context.Modifier))
            {
                result.AddWarning($"Modifier '{context.Modifier}' may not be supported for date parameters");
            }

            return result;
        }
    }
}
