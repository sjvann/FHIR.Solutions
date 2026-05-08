using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public class NumberParameterBuilder : BaseSearchParameterBuilder
    {
        public NumberParameterBuilder(ILogger<NumberParameterBuilder> logger) : base(logger)
        {
        }

        public override string ParameterType => "number";

        public override string BuildParameter(SearchParameterContext context)
        {
            var value = GetValueFromContext(context, "value");
            var prefix = GetValueFromContext(context, "prefix", context.Prefix ?? "");
            
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Number parameter value cannot be empty");
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
                result.AddError("Number parameter value cannot be empty");
                return result;
            }

            // Validate number format
            if (!decimal.TryParse(value, out _))
            {
                result.AddError("Invalid number format");
            }

            // Validate prefix
            var prefix = GetValueFromContext(context, "prefix", context.Prefix ?? "");
            var supportedPrefixes = new[] { "eq", "ne", "gt", "lt", "ge", "le", "ap" };
            if (!string.IsNullOrEmpty(prefix) && !supportedPrefixes.Contains(prefix))
            {
                result.AddError($"Unsupported number prefix: {prefix}");
            }

            // Validate supported modifiers for number parameters
            var supportedModifiers = new[] { "missing" };
            if (!string.IsNullOrEmpty(context.Modifier) && !supportedModifiers.Contains(context.Modifier))
            {
                result.AddWarning($"Modifier '{context.Modifier}' may not be supported for number parameters");
            }

            return result;
        }
    }
}
