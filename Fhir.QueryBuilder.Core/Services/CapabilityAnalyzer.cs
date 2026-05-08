using Fhir.QueryBuilder.Common;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.Resources.R5;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Fhir.QueryBuilder.Services;

/// <summary>基於 R5 <see cref="CapabilityStatement"/> 的輕量分析器。</summary>
public sealed class CapabilityAnalyzer : ICapabilityAnalyzer
{
    private readonly ILogger<CapabilityAnalyzer> _logger;

    public CapabilityAnalyzer(ILogger<CapabilityAnalyzer> logger) => _logger = logger;

    public Task<CapabilityAnalysisResult> AnalyzeCapabilityAsync(CapabilityStatement capabilityStatement)
    {
        var result = new CapabilityAnalysisResult();
        try
        {
            result.FhirVersion = FhirVersionParser.ParseFromCapabilityString((string?)capabilityStatement.FhirVersion);
            result.ServerSoftware = (string?)capabilityStatement.Software?.Name;

            var serverRest = SelectServerRest(capabilityStatement);
            var resources = serverRest?.Resource ?? new List<CapabilityStatement.RestComponent.RestResourceComponent>();

            result.SupportedResources = resources
                .Select(r => (string?)r.Type)
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            result.SupportedInteractions = resources
                .SelectMany(r => r.Interaction ?? new List<CapabilityStatement.RestComponent.RestResourceComponent.RestResourceInteractionComponent>())
                .Select(i => (string?)i.Code)
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            result.TotalSearchParameters = resources.Sum(r => r.SearchParam?.Count ?? 0);

            result.SupportedFormats = capabilityStatement.Format?
                .Select(f => (string?)f)
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .ToList() ?? new List<string>();

            result.Security = new SecurityFeatures { SupportsTls = true };
            result.Advanced = new AdvancedFeatures
            {
                SupportsGraphQL = result.SupportedFormats.Any(x => x.Contains("graphql", StringComparison.OrdinalIgnoreCase)),
                SupportedOperations = result.SupportedInteractions.ToList()
            };

            result.CompatibilityScore = Math.Min(50 + Math.Min(result.SupportedResources.Count * 2, 30) + Math.Min(result.SupportedInteractions.Count, 20), 100);
            result.Recommendations = GenerateRecommendations(result);
            result.PotentialIssues = IdentifyPotentialIssues(result);

            _logger.LogInformation("Analyzed capability: {Resources} resources", result.SupportedResources.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze capability statement");
            result.PotentialIssues.Add($"Analysis failed: {ex.Message}");
        }

        return Task.FromResult(result);
    }

    public async Task<CapabilityComparisonResult> CompareCapabilitiesAsync(CapabilityStatement baseline, CapabilityStatement target)
    {
        var baselineAnalysis = await AnalyzeCapabilityAsync(baseline);
        var targetAnalysis = await AnalyzeCapabilityAsync(target);

        var result = new CapabilityComparisonResult
        {
            BaselineServer = (string?)baseline.Software?.Name ?? "Unknown",
            TargetServer = (string?)target.Software?.Name ?? "Unknown",
            CommonResources = baselineAnalysis.SupportedResources.Intersect(targetAnalysis.SupportedResources, StringComparer.OrdinalIgnoreCase).ToList(),
            BaselineOnlyResources = baselineAnalysis.SupportedResources.Except(targetAnalysis.SupportedResources, StringComparer.OrdinalIgnoreCase).ToList(),
            TargetOnlyResources = targetAnalysis.SupportedResources.Except(baselineAnalysis.SupportedResources, StringComparer.OrdinalIgnoreCase).ToList()
        };

        var totalResources = baselineAnalysis.SupportedResources.Count;
        result.CompatibilityPercentage = totalResources > 0
            ? (double)result.CommonResources.Count / totalResources * 100
            : 0;

        result.FeatureDifferences = CompareFeatures(baselineAnalysis, targetAnalysis);
        result.MigrationRecommendations = GenerateMigrationRecommendations(result);

        return result;
    }

    public async Task<bool> CheckFeatureSupportAsync(CapabilityStatement capabilityStatement, string feature)
    {
        var analysis = await AnalyzeCapabilityAsync(capabilityStatement);
        return feature.ToLowerInvariant() switch
        {
            "oauth" => analysis.Security.SupportsOAuth,
            "smart" => analysis.Security.SupportsSmart,
            "graphql" => analysis.Advanced.SupportsGraphQL,
            "batch" => analysis.Advanced.SupportsBatch,
            "transaction" => analysis.Advanced.SupportsTransaction,
            "subscription" => analysis.Advanced.SupportsSubscription,
            "bulk-data" => analysis.Advanced.SupportsBulkData,
            _ => false
        };
    }

    public Task<QueryStrategyRecommendation> GetQueryStrategyAsync(CapabilityStatement capabilityStatement, string resourceType)
    {
        var serverRest = SelectServerRest(capabilityStatement);
        var resource = serverRest?.Resource?.FirstOrDefault(r =>
            string.Equals((string?)r.Type, resourceType, StringComparison.OrdinalIgnoreCase));

        var recommendation = new QueryStrategyRecommendation { ResourceType = resourceType };

        if (resource != null)
        {
            recommendation.RecommendedSearchParameters = resource.SearchParam?
                .Select(sp => (string?)sp.Name)
                .Where(n => !string.IsNullOrEmpty(n))
                .Cast<string>()
                .Take(10)
                .ToList() ?? new List<string>();

            recommendation.SupportedIncludes = resource.SearchInclude?
                .Select(s => (string?)s)
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>()
                .ToList() ?? new List<string>();

            recommendation.SupportsSorting = resource.Interaction?
                .Any(i => string.Equals((string?)i.Code, "search-type", StringComparison.OrdinalIgnoreCase)) ?? false;
        }
        else
            recommendation.Limitations.Add($"Resource type '{resourceType}' is not supported by this server");

        return Task.FromResult(recommendation);
    }

    private static CapabilityStatement.RestComponent? SelectServerRest(CapabilityStatement? cap)
    {
        if (cap?.Rest == null || cap.Rest.Count == 0)
            return null;
        return cap.Rest.FirstOrDefault(r => string.Equals((string?)r.Mode, "server", StringComparison.OrdinalIgnoreCase))
               ?? cap.Rest[0];
    }

    private static List<string> GenerateRecommendations(CapabilityAnalysisResult analysis)
    {
        var recommendations = new List<string>();
        if (analysis.SupportedResources.Count < 10)
            recommendations.Add("Server supports limited resources. Consider using a more complete FHIR implementation.");
        if (analysis.TotalSearchParameters < 50)
            recommendations.Add("Limited search parameter support. Query capabilities may be restricted.");
        return recommendations;
    }

    private static List<string> IdentifyPotentialIssues(CapabilityAnalysisResult analysis)
    {
        var issues = new List<string>();
        if (analysis.FhirVersion == null || analysis.FhirVersion == FhirVersion.Unknown)
            issues.Add("Unable to determine FHIR version");
        if (!analysis.SupportedFormats.Contains("application/fhir+json", StringComparer.OrdinalIgnoreCase))
            issues.Add("JSON format support not explicitly declared");
        return issues;
    }

    private static List<FeatureDifference> CompareFeatures(CapabilityAnalysisResult baseline, CapabilityAnalysisResult target)
    {
        var differences = new List<FeatureDifference>();
        if (baseline.Security.SupportsOAuth != target.Security.SupportsOAuth)
        {
            differences.Add(new FeatureDifference
            {
                FeatureName = "OAuth Support",
                BaselineSupported = baseline.Security.SupportsOAuth,
                TargetSupported = target.Security.SupportsOAuth,
                Impact = ImpactLevel.Medium,
                Description = "OAuth authentication support differs between servers"
            });
        }

        return differences;
    }

    private static List<string> GenerateMigrationRecommendations(CapabilityComparisonResult comparison)
    {
        var recommendations = new List<string>();
        if (comparison.CompatibilityPercentage < 80)
            recommendations.Add("Low compatibility detected. Thorough testing recommended before migration.");
        if (comparison.BaselineOnlyResources.Any())
            recommendations.Add($"The following resources are not available in target server: {string.Join(", ", comparison.BaselineOnlyResources)}");
        return recommendations;
    }
}
