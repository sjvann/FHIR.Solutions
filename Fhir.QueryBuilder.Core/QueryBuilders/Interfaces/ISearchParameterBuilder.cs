using Fhir.QueryBuilder.Services.Interfaces;

namespace Fhir.QueryBuilder.QueryBuilders.Interfaces
{
    public class SearchParameterContext
    {
        public string ParameterName { get; set; } = string.Empty;
        public string ParameterType { get; set; } = string.Empty;
        public Dictionary<string, object> Values { get; set; } = new();
        public string? Modifier { get; set; }
        public string? Prefix { get; set; }
    }

    public interface ISearchParameterBuilder
    {
        string ParameterType { get; }
        bool CanHandle(string parameterType);
        string BuildParameter(SearchParameterContext context);
        ValidationResult ValidateParameter(SearchParameterContext context);
    }
}
