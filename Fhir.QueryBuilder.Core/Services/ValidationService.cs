using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace Fhir.QueryBuilder.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> _logger;
        private readonly HttpClient _httpClient;
        private static readonly Regex UrlRegex = new(@"^https?://[^\s/$.?#].[^\s]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex FhirDateRegex = new(@"^\d{4}(-\d{2}(-\d{2})?)?$", RegexOptions.Compiled);
        private static readonly Regex FhirDateTimeRegex = new(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{3})?(Z|[+-]\d{2}:\d{2})$", RegexOptions.Compiled);
        private static readonly string[] SupportedFhirVersions = { "R4", "R5" };

        public ValidationService(ILogger<ValidationService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public ValidationResult ValidateUrl(string url)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(url))
            {
                result.AddError("URL cannot be empty or whitespace");
                return result;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                result.AddError("Invalid URL format");
                return result;
            }

            if (uri.Scheme != "https" && uri.Scheme != "http")
            {
                result.AddError("URL must use HTTP or HTTPS protocol");
            }

            if (!UrlRegex.IsMatch(url))
            {
                result.AddError("URL format is not valid");
            }

            // Check for common FHIR server patterns
            if (!url.Contains("fhir", StringComparison.OrdinalIgnoreCase))
            {
                result.AddWarning("URL does not appear to be a FHIR server endpoint");
            }

            _logger.LogDebug("URL validation result for {Url}: {IsValid}", url, result.IsValid);
            return result;
        }

        public ValidationResult ValidateSearchParameter(string parameterName, string parameterValue, string parameterType)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                result.AddError("Parameter name cannot be empty");
                return result;
            }

            if (string.IsNullOrWhiteSpace(parameterValue))
            {
                result.AddError("Parameter value cannot be empty");
                return result;
            }

            // Validate based on parameter type
            switch (parameterType.ToLowerInvariant())
            {
                case "date":
                    ValidateDateParameter(parameterValue, result);
                    break;
                case "number":
                    ValidateNumberParameter(parameterValue, result);
                    break;
                case "quantity":
                    ValidateQuantityParameter(parameterValue, result);
                    break;
                case "reference":
                    ValidateReferenceParameter(parameterValue, result);
                    break;
                case "string":
                    ValidateStringParameter(parameterValue, result);
                    break;
                case "token":
                    ValidateTokenParameter(parameterValue, result);
                    break;
                case "uri":
                    ValidateUriParameter(parameterValue, result);
                    break;
                case "composite":
                    ValidateCompositeParameter(parameterValue, result);
                    break;
                case "special":
                    ValidateSpecialParameter(parameterValue, result);
                    break;
                default:
                    result.AddWarning($"Unknown parameter type: {parameterType}");
                    break;
            }

            _logger.LogDebug("Parameter validation result for {ParameterName}={ParameterValue} ({ParameterType}): {IsValid}", 
                parameterName, parameterValue, parameterType, result.IsValid);
            return result;
        }

        public ValidationResult ValidateQuery(string query)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(query))
            {
                result.AddError("Query cannot be empty");
                return result;
            }

            if (!Uri.TryCreate(query, UriKind.Absolute, out var uri))
            {
                result.AddError("Query is not a valid URL");
                return result;
            }

            // Basic FHIR query structure validation
            var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (pathSegments.Length == 0)
            {
                result.AddError("Query must specify a resource type");
            }

            _logger.LogDebug("Query validation result for {Query}: {IsValid}", query, result.IsValid);
            return result;
        }

        public ValidationResult ValidateResourceType(string resourceType, IEnumerable<string>? supportedResources = null)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(resourceType))
            {
                result.AddError("Resource type cannot be empty");
                return result;
            }

            if (!char.IsUpper(resourceType[0]))
                result.AddError("Resource type must start with an uppercase letter (FHIR resource name)");

            if (supportedResources != null && !supportedResources.Contains(resourceType))
            {
                result.AddError($"Resource type '{resourceType}' is not supported by the server");
            }

            _logger.LogDebug("Resource type validation result for {ResourceType}: {IsValid}", resourceType, result.IsValid);
            return result;
        }

        private void ValidateDateParameter(string value, ValidationResult result)
        {
            // Remove prefixes like ge, le, gt, lt, eq, ne, sa, eb, ap
            var cleanValue = value;
            var prefixes = new[] { "ge", "le", "gt", "lt", "eq", "ne", "sa", "eb", "ap" };
            foreach (var prefix in prefixes)
            {
                if (value.StartsWith(prefix))
                {
                    cleanValue = value.Substring(prefix.Length);
                    break;
                }
            }

            if (!FhirDateRegex.IsMatch(cleanValue) && !FhirDateTimeRegex.IsMatch(cleanValue))
            {
                result.AddError("Invalid date format. Use YYYY, YYYY-MM, YYYY-MM-DD, or full datetime format");
            }
        }

        private void ValidateNumberParameter(string value, ValidationResult result)
        {
            // Remove prefixes
            var cleanValue = value;
            var prefixes = new[] { "ge", "le", "gt", "lt", "eq", "ne", "ap" };
            foreach (var prefix in prefixes)
            {
                if (value.StartsWith(prefix))
                {
                    cleanValue = value.Substring(prefix.Length);
                    break;
                }
            }

            if (!decimal.TryParse(cleanValue, out _))
            {
                result.AddError("Invalid number format");
            }
        }

        private void ValidateQuantityParameter(string value, ValidationResult result)
        {
            // Quantity can be: [prefix]number|system|code or [prefix]number||code
            var parts = value.Split('|');
            if (parts.Length > 3)
            {
                result.AddError("Invalid quantity format. Use [prefix]number|system|code");
                return;
            }

            var numberPart = parts[0];
            // Remove prefixes
            var prefixes = new[] { "ge", "le", "gt", "lt", "eq", "ne", "ap" };
            foreach (var prefix in prefixes)
            {
                if (numberPart.StartsWith(prefix))
                {
                    numberPart = numberPart.Substring(prefix.Length);
                    break;
                }
            }

            if (!decimal.TryParse(numberPart, out _))
            {
                result.AddError("Invalid number in quantity parameter");
            }
        }

        private void ValidateReferenceParameter(string value, ValidationResult result)
        {
            // Reference can be: ResourceType/id, id, or full URL
            if (value.Contains('/'))
            {
                var parts = value.Split('/');
                if (parts.Length != 2)
                {
                    result.AddError("Invalid reference format. Use ResourceType/id");
                }
            }
        }

        private void ValidateStringParameter(string value, ValidationResult result)
        {
            // String parameters are generally flexible, just check for reasonable length
            if (value.Length > 1000)
            {
                result.AddWarning("String parameter is very long and might cause performance issues");
            }
        }

        private void ValidateTokenParameter(string value, ValidationResult result)
        {
            // Token can be: code, system|code, or |code
            if (value.Contains('|'))
            {
                var parts = value.Split('|');
                if (parts.Length != 2)
                {
                    result.AddError("Invalid token format. Use system|code or |code");
                }
            }
        }

        private void ValidateUriParameter(string value, ValidationResult result)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                result.AddError("Invalid URI format");
            }
        }

        private void ValidateCompositeParameter(string value, ValidationResult result)
        {
            var parts = value.Split('$', StringSplitOptions.None);
            if (parts.Length < 2)
            {
                result.AddError("Composite search value must contain at least two components separated by '$'");
                return;
            }

            foreach (var p in parts)
            {
                if (string.IsNullOrWhiteSpace(p))
                    result.AddWarning("Empty composite component");
            }
        }

        private void ValidateSpecialParameter(string value, ValidationResult result)
        {
            if (value.Length > 2000)
                result.AddWarning("Special search parameter value is very long; format is server-specific");
        }

        public ValidationResult ValidateConnectionSettings(string serverUrl, string? token = null)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate server URL
            var urlValidation = ValidateUrl(serverUrl);
            if (!urlValidation.IsValid)
            {
                result.Errors.AddRange(urlValidation.Errors);
                result.Warnings.AddRange(urlValidation.Warnings);
                result.IsValid = false;
            }

            // Validate token format if provided
            if (!string.IsNullOrEmpty(token))
            {
                if (token.Length < 10)
                {
                    result.AddWarning("Token appears to be very short");
                }

                // Basic JWT token format check
                if (token.Split('.').Length == 3)
                {
                    _logger.LogDebug("Token appears to be in JWT format");
                }
                else if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    result.AddWarning("Token may need 'Bearer ' prefix");
                }
            }

            _logger.LogDebug("Connection settings validation result: {IsValid}", result.IsValid);
            return result;
        }

        public ValidationResult ValidateQueryComplexity(string query)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrEmpty(query))
            {
                result.AddError("Query cannot be empty");
                return result;
            }

            try
            {
                var uri = new Uri(query);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);

                // Check for too many parameters
                if (queryParams.Count > 20)
                {
                    result.AddWarning("Query has many parameters and may be slow");
                }

                // Check for complex search patterns
                foreach (string key in queryParams.Keys)
                {
                    if (key == null) continue;

                    var value = queryParams[key];
                    if (string.IsNullOrEmpty(value)) continue;

                    // Check for chained parameters
                    if (key.Contains('.'))
                    {
                        result.AddWarning($"Chained parameter '{key}' may impact performance");
                    }

                    // Check for complex modifiers
                    if (key.Contains(':'))
                    {
                        var parts = key.Split(':');
                        if (parts.Length > 2)
                        {
                            result.AddWarning($"Complex modifier in parameter '{key}'");
                        }
                    }

                    // Check for large result sets
                    if (key.Equals("_count", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(value, out var count) && count > 1000)
                        {
                            result.AddWarning("Large result set requested - may cause timeout");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to parse query: {ex.Message}");
            }

            _logger.LogDebug("Query complexity validation result: {IsValid}", result.IsValid);
            return result;
        }

        public ValidationResult ValidateFhirVersion(string version)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(version))
            {
                result.AddError("FHIR version cannot be empty");
                return result;
            }

            if (!SupportedFhirVersions.Contains(version, StringComparer.OrdinalIgnoreCase))
            {
                result.AddWarning($"FHIR version '{version}' may not be fully supported. Supported versions: {string.Join(", ", SupportedFhirVersions)}");
            }

            _logger.LogDebug("FHIR version validation result for {Version}: {IsValid}", version, result.IsValid);
            return result;
        }

        public async Task<ValidationResult> ValidateServerCapabilityAsync(string serverUrl, CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult { IsValid = true };

            try
            {
                var metadataUrl = serverUrl.TrimEnd('/') + "/metadata";
                _logger.LogDebug("Checking server capability at: {MetadataUrl}", metadataUrl);

                using var response = await _httpClient.GetAsync(metadataUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    result.AddError($"Server capability check failed: {response.StatusCode} - {response.ReasonPhrase}");
                    return result;
                }

                var contentType = response.Content.Headers.ContentType?.MediaType;
                if (!string.IsNullOrEmpty(contentType))
                {
                    if (!contentType.Contains("json") && !contentType.Contains("xml"))
                    {
                        result.AddWarning($"Unexpected content type: {contentType}");
                    }
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrEmpty(content))
                {
                    result.AddError("Server returned empty capability statement");
                }
                else if (!content.Contains("CapabilityStatement"))
                {
                    result.AddWarning("Response may not be a valid FHIR CapabilityStatement");
                }

                _logger.LogDebug("Server capability validation completed successfully");
            }
            catch (HttpRequestException ex)
            {
                result.AddError($"Network error during capability check: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                result.AddError("Server capability check timed out");
            }
            catch (Exception ex)
            {
                result.AddError($"Unexpected error during capability check: {ex.Message}");
            }

            return result;
        }
    }
}
