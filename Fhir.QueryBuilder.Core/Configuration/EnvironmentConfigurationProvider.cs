using System.Collections;
using Microsoft.Extensions.Configuration;

namespace Fhir.QueryBuilder.Configuration
{
    public class EnvironmentConfigurationProvider : ConfigurationProvider
    {
        private readonly string _prefix;

        public EnvironmentConfigurationProvider(string prefix = "FHIR_")
        {
            _prefix = prefix;
        }

        public override void Load()
        {
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
            {
                var key = envVar.Key?.ToString();
                var value = envVar.Value?.ToString();

                if (string.IsNullOrEmpty(key) || !key.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Remove prefix and convert to configuration key format
                var configKey = key.Substring(_prefix.Length);
                configKey = ConvertToConfigurationKey(configKey);

                data[configKey] = value;
            }

            Data = data;
        }

        private static string ConvertToConfigurationKey(string environmentKey)
        {
            // Convert FHIR_QUERY_BUILDER_DEFAULT_SERVER_URL to Fhir.QueryBuilder:DefaultServerUrl
            var parts = environmentKey.Split('_', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
                return environmentKey;

            var result = new List<string>();
            
            // First part becomes the section name
            if (parts.Length > 0)
            {
                result.Add(ConvertToPascalCase(string.Join("", parts.Take(2)))); // QUERY_BUILDER -> QueryBuilder
            }

            // Remaining parts become nested keys
            for (int i = 2; i < parts.Length; i++)
            {
                result.Add(ConvertToPascalCase(parts[i]));
            }

            return string.Join(":", result);
        }

        private static string ConvertToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var words = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
            var result = new System.Text.StringBuilder();

            foreach (var word in words)
            {
                if (word.Length > 0)
                {
                    result.Append(char.ToUpper(word[0]));
                    if (word.Length > 1)
                    {
                        result.Append(word.Substring(1).ToLower());
                    }
                }
            }

            return result.ToString();
        }
    }

    public class EnvironmentConfigurationSource : IConfigurationSource
    {
        private readonly string _prefix;

        public EnvironmentConfigurationSource(string prefix = "FHIR_")
        {
            _prefix = prefix;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EnvironmentConfigurationProvider(_prefix);
        }
    }

    public static class EnvironmentConfigurationExtensions
    {
        public static IConfigurationBuilder AddFhirEnvironmentVariables(
            this IConfigurationBuilder builder, 
            string prefix = "FHIR_")
        {
            return builder.Add(new EnvironmentConfigurationSource(prefix));
        }
    }
}
