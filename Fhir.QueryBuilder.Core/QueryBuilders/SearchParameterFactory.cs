using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.QueryBuilders
{
    public class SearchParameterFactory : ISearchParameterFactory
    {
        private readonly IEnumerable<ISearchParameterBuilder> _builders;
        private readonly ILogger<SearchParameterFactory> _logger;

        public SearchParameterFactory(IEnumerable<ISearchParameterBuilder> builders, ILogger<SearchParameterFactory> logger)
        {
            _builders = builders;
            _logger = logger;
        }

        public ISearchParameterBuilder? GetBuilder(string parameterType)
        {
            var builder = _builders.FirstOrDefault(b => b.CanHandle(parameterType));
            
            if (builder == null)
            {
                _logger.LogWarning("No builder found for parameter type: {ParameterType}", parameterType);
            }
            else
            {
                _logger.LogDebug("Found builder {BuilderType} for parameter type: {ParameterType}", 
                    builder.GetType().Name, parameterType);
            }

            return builder;
        }

        public IEnumerable<ISearchParameterBuilder> GetAllBuilders()
        {
            return _builders;
        }

        public IEnumerable<string> GetSupportedParameterTypes()
        {
            return _builders.Select(b => b.ParameterType).Distinct().OrderBy(t => t);
        }
    }
}
