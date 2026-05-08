using Fhir.QueryBuilder.Common;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.Resources.R5;
using Fhir.TypeFramework.Serialization;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Fhir.QueryBuilder.Services;

public sealed class ServerCompatibilityService : IServerCompatibilityService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ServerCompatibilityService> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(24);

    public ServerCompatibilityService(
        HttpClient httpClient,
        ICacheService cacheService,
        ILogger<ServerCompatibilityService> logger)
    {
        _httpClient = httpClient;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ServerCompatibilityReport> CheckCompatibilityAsync(string serverUrl)
    {
        var cacheKey = $"compatibility:{serverUrl}";
        var cached = await _cacheService.GetAsync<ServerCompatibilityReport>(cacheKey);
        if (cached != null)
            return cached;

        var report = new ServerCompatibilityReport { ServerUrl = serverUrl, CheckedAt = DateTime.UtcNow };

        try
        {
            var capabilityStatement = await GetCapabilityStatementAsync(serverUrl);
            if (capabilityStatement == null)
            {
                report.IsAvailable = false;
                report.ErrorMessage = "Unable to retrieve CapabilityStatement";
                return report;
            }

            report.IsAvailable = true;
            report.DetectedVersion = FhirVersionParser.ParseFromCapabilityString((string?)capabilityStatement.FhirVersion);
            report.ServerSoftware = (string?)capabilityStatement.Software?.Name;
            report.ImplementationGuides = capabilityStatement.ImplementationGuide?
                .Select(c => (string?)c)
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .ToList() ?? new List<string>();

            var serverRest = SelectServerRest(capabilityStatement);
            foreach (var restResource in serverRest?.Resource ?? Enumerable.Empty<CapabilityStatement.RestComponent.RestResourceComponent>())
            {
                var typeName = (string?)restResource.Type ?? "";
                if (string.IsNullOrEmpty(typeName))
                    continue;

                report.ResourceSupport.Add(new ResourceSupportInfo
                {
                    Type = typeName,
                    Interactions = restResource.Interaction?
                        .Select(i => (string?)i.Code)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Cast<string>()
                        .ToList() ?? new List<string>(),
                    SearchParameterCount = restResource.SearchParam?.Count ?? 0,
                    CompatibilityScore = CalculateResourceCompatibilityScore(restResource)
                });
            }

            report.OverallCompatibilityScore = CalculateOverallCompatibilityScore(report);
            report.Recommendations = GenerateRecommendations(report);

            await _cacheService.SetAsync(cacheKey, report, _cacheExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check compatibility for {ServerUrl}", serverUrl);
            report.IsAvailable = false;
            report.ErrorMessage = ex.Message;
        }

        return report;
    }

    public async Task<ResourceSupportReport> CheckResourceSupportAsync(string serverUrl, string resourceType)
    {
        var cacheKey = $"resource:{serverUrl}:{resourceType}";
        var cached = await _cacheService.GetAsync<ResourceSupportReport>(cacheKey);
        if (cached != null)
            return cached;

        var report = new ResourceSupportReport { ResourceType = resourceType };

        try
        {
            var capabilityStatement = await GetCapabilityStatementAsync(serverUrl);
            if (capabilityStatement == null)
            {
                report.ErrorMessage = "Unable to retrieve CapabilityStatement";
                return report;
            }

            var serverRest = SelectServerRest(capabilityStatement);
            var restResource = serverRest?.Resource?.FirstOrDefault(r =>
                string.Equals((string?)r.Type, resourceType, StringComparison.OrdinalIgnoreCase));

            if (restResource == null)
            {
                report.IsSupported = false;
                return report;
            }

            report.IsSupported = true;
            report.SupportedInteractions = restResource.Interaction?
                .Select(i => (string?)i.Code)
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .ToList() ?? new List<string>();

            report.SupportedIncludes = restResource.SearchInclude?
                .Select(s => (string?)s)
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .ToList() ?? new List<string>();

            report.SupportedRevIncludes = restResource.SearchRevInclude?
                .Select(s => (string?)s)
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .ToList() ?? new List<string>();

            report.SearchParameters = restResource.SearchParam?.Select(sp => new SearchParameterSupportInfo
            {
                Name = (string?)sp.Name ?? "",
                Type = ParseSearchParameterType((string?)sp.Type),
                Modifiers = new List<string>(),
                IsStandard = IsStandardParameter((string?)sp.Name ?? "")
            }).ToList() ?? new List<SearchParameterSupportInfo>();

            await _cacheService.SetAsync(cacheKey, report, _cacheExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check resource support for {ResourceType} on {ServerUrl}",
                resourceType, serverUrl);
            report.ErrorMessage = ex.Message;
        }

        return report;
    }

    public async Task<SearchParameterSupportReport> CheckSearchParameterSupportAsync(
        string serverUrl, string resourceType, string searchParameter)
    {
        var resourceReport = await CheckResourceSupportAsync(serverUrl, resourceType);

        var paramInfo = resourceReport.SearchParameters
            .FirstOrDefault(sp => sp.Name.Equals(searchParameter, StringComparison.OrdinalIgnoreCase));

        return new SearchParameterSupportReport
        {
            ParameterName = searchParameter,
            IsSupported = paramInfo != null,
            Type = paramInfo?.Type ?? SearchParameterType.String,
            SupportedModifiers = paramInfo?.Modifiers ?? new List<string>(),
            ErrorMessage = resourceReport.ErrorMessage
        };
    }

    public Task<OperationSupportReport> CheckOperationSupportAsync(string serverUrl, string operation)
    {
        return Task.FromResult(new OperationSupportReport
        {
            OperationName = operation,
            IsSupported = false
        });
    }

    public async Task<CapabilityStatement?> GetCapabilityStatementAsync(string serverUrl)
    {
        var cacheKey = $"capability:{serverUrl}";
        var cached = await _cacheService.GetAsync<CapabilityStatement>(cacheKey);
        if (cached != null)
            return cached;

        try
        {
            var url = $"{serverUrl.TrimEnd('/')}/metadata";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to retrieve CapabilityStatement from {Url}, status: {Status}",
                    url, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var capability = FhirJsonSerializer.Deserialize<CapabilityStatement>(json);

            if (capability != null)
                await _cacheService.SetAsync(cacheKey, capability, _cacheExpiration);

            return capability;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get CapabilityStatement from {ServerUrl}", serverUrl);
            return null;
        }
    }

    public async Task<FhirVersion?> DetectFhirVersionAsync(string serverUrl)
    {
        var capability = await GetCapabilityStatementAsync(serverUrl);
        return capability != null
            ? FhirVersionParser.ParseFromCapabilityString((string?)capability.FhirVersion)
            : null;
    }

    public async Task ClearCacheAsync(string? serverUrl = null)
    {
        if (string.IsNullOrEmpty(serverUrl))
            await _cacheService.ClearAsync();
        else
        {
            await _cacheService.RemoveByPatternAsync($"compatibility:{serverUrl}");
            await _cacheService.RemoveByPatternAsync($"capability:{serverUrl}");
            await _cacheService.RemoveByPatternAsync($"resource:{serverUrl}:*");
        }
    }

    private static CapabilityStatement.RestComponent? SelectServerRest(CapabilityStatement? cap)
    {
        if (cap?.Rest == null || cap.Rest.Count == 0)
            return null;
        return cap.Rest.FirstOrDefault(r => string.Equals((string?)r.Mode, "server", StringComparison.OrdinalIgnoreCase))
               ?? cap.Rest[0];
    }

    private static SearchParameterType ParseSearchParameterType(string? typeString)
    {
        return typeString?.ToLowerInvariant() switch
        {
            "number" => SearchParameterType.Number,
            "date" => SearchParameterType.Date,
            "string" => SearchParameterType.String,
            "token" => SearchParameterType.Token,
            "reference" => SearchParameterType.Reference,
            "composite" => SearchParameterType.Composite,
            "quantity" => SearchParameterType.Quantity,
            "uri" => SearchParameterType.Uri,
            "special" => SearchParameterType.Special,
            _ => SearchParameterType.String
        };
    }

    private static bool IsStandardParameter(string parameterName)
    {
        var standardParams = new[] { "_id", "_lastUpdated", "_profile", "_security", "_tag", "_source" };
        return standardParams.Contains(parameterName);
    }

    private static int CalculateResourceCompatibilityScore(CapabilityStatement.RestComponent.RestResourceComponent resource)
    {
        var score = 50;
        var interactions = resource.Interaction?
            .Select(i => (string?)i.Code)
            .Where(s => !string.IsNullOrEmpty(s))
            .Cast<string>()
            .ToList() ?? new List<string>();

        if (interactions.Contains("read")) score += 10;
        if (interactions.Contains("search-type")) score += 15;
        if (interactions.Contains("create")) score += 10;
        if (interactions.Contains("update")) score += 10;
        if (interactions.Contains("delete")) score += 5;

        score += Math.Min((resource.SearchParam?.Count ?? 0) * 2, 20);

        return Math.Min(score, 100);
    }

    private static int CalculateOverallCompatibilityScore(ServerCompatibilityReport report)
    {
        if (!report.ResourceSupport.Any())
            return 0;

        return (int)report.ResourceSupport.Average(r => r.CompatibilityScore);
    }

    private static List<string> GenerateRecommendations(ServerCompatibilityReport report)
    {
        var recommendations = new List<string>();

        if (report.DetectedVersion == null || report.DetectedVersion == FhirVersion.Unknown)
            recommendations.Add("Unable to detect FHIR version. Ensure server provides valid CapabilityStatement.");

        if (report.OverallCompatibilityScore < 70)
            recommendations.Add("Server compatibility score is low. Consider using a more complete FHIR server implementation.");

        var resourcesWithLowScore = report.ResourceSupport.Where(r => r.CompatibilityScore < 60).ToList();
        if (resourcesWithLowScore.Any())
            recommendations.Add($"The following resources have limited support: {string.Join(", ", resourcesWithLowScore.Select(r => r.Type))}");

        return recommendations;
    }
}
