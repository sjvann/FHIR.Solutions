using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public class StringParameterBuilder : BaseSearchParameterBuilder
    {
        public StringParameterBuilder(ILogger<StringParameterBuilder> logger) : base(logger)
        {
        }

        public override string ParameterType => "string";

        public override string BuildParameter(SearchParameterContext context)
        {
            var value = GetValueFromContext(context, "value");
            
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("String parameter value cannot be empty");
            }

            var result = BuildParameterString(context.ParameterName, value, context.Modifier);
            LogParameterBuilding(context, result);
            return result;
        }

        public override ValidationResult ValidateParameter(SearchParameterContext context)
        {
            var result = base.ValidateParameter(context);
            
            var value = GetValueFromContext(context, "value");
            if (string.IsNullOrEmpty(value))
            {
                result.AddError("String parameter value cannot be empty");
            }

            if (value.Length > 1000)
            {
                result.AddWarning("String parameter is very long and might cause performance issues");
            }

            // Validate supported modifiers for string parameters
            var supportedModifiers = new[] { "exact", "contains", "missing" };
            if (!string.IsNullOrEmpty(context.Modifier) && !supportedModifiers.Contains(context.Modifier))
            {
                result.AddWarning($"Modifier '{context.Modifier}' may not be supported for string parameters");
            }

            return result;
        }
    }
}
