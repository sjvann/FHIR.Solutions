using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public class UriParameterBuilder : BaseSearchParameterBuilder
    {
        public UriParameterBuilder(ILogger<UriParameterBuilder> logger) : base(logger)
        {
        }

        public override string ParameterType => "uri";

        public override string BuildParameter(SearchParameterContext context)
        {
            var value = GetValueFromContext(context, "value");
            
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("URI parameter value cannot be empty");
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
                result.AddError("URI parameter value cannot be empty");
                return result;
            }

            // Validate URI format
            if (!Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                result.AddError("Invalid URI format");
            }

            // Validate supported modifiers for URI parameters
            var supportedModifiers = new[] { "below", "above", "missing" };
            if (!string.IsNullOrEmpty(context.Modifier) && !supportedModifiers.Contains(context.Modifier))
            {
                result.AddWarning($"Modifier '{context.Modifier}' may not be supported for URI parameters");
            }

            return result;
        }
    }
}
