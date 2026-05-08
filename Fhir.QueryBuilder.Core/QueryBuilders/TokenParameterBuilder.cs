using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public class TokenParameterBuilder : BaseSearchParameterBuilder
    {
        public TokenParameterBuilder(ILogger<TokenParameterBuilder> logger) : base(logger)
        {
        }

        public override string ParameterType => "token";

        public override string BuildParameter(SearchParameterContext context)
        {
            var code = GetValueFromContext(context, "code");
            var system = GetValueFromContext(context, "system");
            
            string parameterValue;
            
            if (!string.IsNullOrEmpty(system) && !string.IsNullOrEmpty(code))
            {
                // System|Code format
                parameterValue = $"{system}|{code}";
            }
            else if (!string.IsNullOrEmpty(system))
            {
                // System| format (any code in the system)
                parameterValue = $"{system}|";
            }
            else if (!string.IsNullOrEmpty(code))
            {
                // |Code format (code in any system) or just Code
                parameterValue = string.IsNullOrEmpty(system) ? code : $"|{code}";
            }
            else
            {
                throw new ArgumentException("Token parameter must have at least code or system");
            }

            var result = BuildParameterString(context.ParameterName, parameterValue, context.Modifier);
            LogParameterBuilding(context, result);
            return result;
        }

        public override ValidationResult ValidateParameter(SearchParameterContext context)
        {
            var result = base.ValidateParameter(context);
            
            var code = GetValueFromContext(context, "code");
            var system = GetValueFromContext(context, "system");
            
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(system))
            {
                result.AddError("Token parameter must have at least code or system");
            }

            // Validate system URI format if provided
            if (!string.IsNullOrEmpty(system) && !Uri.TryCreate(system, UriKind.Absolute, out _))
            {
                result.AddWarning("System should be a valid URI");
            }

            // Validate supported modifiers for token parameters
            var supportedModifiers = new[] { "text", "not", "above", "below", "in", "not-in", "missing" };
            if (!string.IsNullOrEmpty(context.Modifier) && !supportedModifiers.Contains(context.Modifier))
            {
                result.AddWarning($"Modifier '{context.Modifier}' may not be supported for token parameters");
            }

            return result;
        }
    }
}
