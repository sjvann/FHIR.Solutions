using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public class QuantityParameterBuilder : BaseSearchParameterBuilder
    {
        public QuantityParameterBuilder(ILogger<QuantityParameterBuilder> logger) : base(logger)
        {
        }

        public override string ParameterType => "quantity";

        public override string BuildParameter(SearchParameterContext context)
        {
            var number = GetValueFromContext(context, "number");
            var system = GetValueFromContext(context, "system");
            var code = GetValueFromContext(context, "code");
            var prefix = GetValueFromContext(context, "prefix", context.Prefix ?? "");
            
            if (string.IsNullOrEmpty(number))
            {
                throw new ArgumentException("Quantity parameter must have a number value");
            }

            string parameterValue;
            
            if (!string.IsNullOrEmpty(system) && !string.IsNullOrEmpty(code))
            {
                // number|system|code format
                parameterValue = $"{prefix}{number}|{system}|{code}";
            }
            else if (!string.IsNullOrEmpty(code))
            {
                // number||code format (no system)
                parameterValue = $"{prefix}{number}||{code}";
            }
            else
            {
                // just number
                parameterValue = $"{prefix}{number}";
            }

            var result = BuildParameterString(context.ParameterName, parameterValue, context.Modifier);
            LogParameterBuilding(context, result);
            return result;
        }

        public override ValidationResult ValidateParameter(SearchParameterContext context)
        {
            var result = base.ValidateParameter(context);
            
            var number = GetValueFromContext(context, "number");
            if (string.IsNullOrEmpty(number))
            {
                result.AddError("Quantity parameter must have a number value");
                return result;
            }

            // Validate number format
            if (!decimal.TryParse(number, out _))
            {
                result.AddError("Invalid number format in quantity parameter");
            }

            // Validate prefix
            var prefix = GetValueFromContext(context, "prefix", context.Prefix ?? "");
            var supportedPrefixes = new[] { "eq", "ne", "gt", "lt", "ge", "le", "ap" };
            if (!string.IsNullOrEmpty(prefix) && !supportedPrefixes.Contains(prefix))
            {
                result.AddError($"Unsupported quantity prefix: {prefix}");
            }

            // Validate system URI format if provided
            var system = GetValueFromContext(context, "system");
            if (!string.IsNullOrEmpty(system) && !Uri.TryCreate(system, UriKind.Absolute, out _))
            {
                result.AddWarning("Quantity system should be a valid URI");
            }

            // Validate supported modifiers for quantity parameters
            var supportedModifiers = new[] { "missing" };
            if (!string.IsNullOrEmpty(context.Modifier) && !supportedModifiers.Contains(context.Modifier))
            {
                result.AddWarning($"Modifier '{context.Modifier}' may not be supported for quantity parameters");
            }

            return result;
        }
    }
}
