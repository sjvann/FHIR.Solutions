using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public class ReferenceParameterBuilder : BaseSearchParameterBuilder
    {
        public ReferenceParameterBuilder(ILogger<ReferenceParameterBuilder> logger) : base(logger)
        {
        }

        public override string ParameterType => "reference";

        public override string BuildParameter(SearchParameterContext context)
        {
            var resourceType = GetValueFromContext(context, "resourceType");
            var id = GetValueFromContext(context, "id");
            var url = GetValueFromContext(context, "url");
            
            string parameterValue;
            
            if (!string.IsNullOrEmpty(url))
            {
                // Full URL reference
                parameterValue = url;
            }
            else if (!string.IsNullOrEmpty(resourceType) && !string.IsNullOrEmpty(id))
            {
                // ResourceType/id format
                parameterValue = $"{resourceType}/{id}";
            }
            else if (!string.IsNullOrEmpty(id))
            {
                // Just id (relative reference)
                parameterValue = id;
            }
            else
            {
                throw new ArgumentException("Reference parameter must have id, resourceType/id, or full URL");
            }

            var result = BuildParameterString(context.ParameterName, parameterValue, context.Modifier);
            LogParameterBuilding(context, result);
            return result;
        }

        public override ValidationResult ValidateParameter(SearchParameterContext context)
        {
            var result = base.ValidateParameter(context);
            
            var resourceType = GetValueFromContext(context, "resourceType");
            var id = GetValueFromContext(context, "id");
            var url = GetValueFromContext(context, "url");
            
            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(id))
            {
                result.AddError("Reference parameter must have id, resourceType/id, or full URL");
            }

            // Validate URL format if provided
            if (!string.IsNullOrEmpty(url) && !Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                result.AddError("Reference URL must be a valid absolute URI");
            }

            // Validate resource type format if provided
            if (!string.IsNullOrEmpty(resourceType) && !char.IsUpper(resourceType[0]))
            {
                result.AddWarning("Resource type should start with uppercase letter");
            }

            // Validate supported modifiers for reference parameters
            var supportedModifiers = new[] { "identifier", "missing" };
            if (!string.IsNullOrEmpty(context.Modifier) && !supportedModifiers.Contains(context.Modifier))
            {
                result.AddWarning($"Modifier '{context.Modifier}' may not be supported for reference parameters");
            }

            return result;
        }
    }
}
