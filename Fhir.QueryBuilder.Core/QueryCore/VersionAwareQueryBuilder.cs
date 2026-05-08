using Fhir.QueryBuilder.Common;
using Fhir.QueryBuilder.Metadata;
using Fhir.QueryBuilder.QueryCore.Validation;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.QueryCore;

/// <summary>
/// 版本感知查詢建構器實作（metadata 來自連線之 CapabilityStatement）。
/// </summary>
public sealed class VersionAwareQueryBuilder : IVersionAwareQueryBuilder
{
    private readonly IMetadataResourceProviderResolver _metadataResourceProviderResolver;
    private readonly IServerCompatibilityService _compatibilityService;
    private readonly ILogger<VersionAwareQueryBuilder> _logger;

    private FhirVersion _targetVersion = FhirVersion.R5;
    private string _resourceType = string.Empty;
    private string _serverUrl = string.Empty;
    private readonly List<QueryParameter> _parameters = new();
    private readonly List<string> _includes = new();
    private readonly List<string> _revIncludes = new();
    private readonly List<SortParameter> _sortParameters = new();
    private int _count = 50;
    private int _offset = 0;
    private bool _pagingExplicitlySet;
    private bool _compatibilityCheckEnabled = true;

    private IMetadataResourceProvider? _resourceProvider;

    public VersionAwareQueryBuilder(
        IMetadataResourceProviderResolver metadataResourceProviderResolver,
        IServerCompatibilityService compatibilityService,
        ILogger<VersionAwareQueryBuilder> logger)
    {
        _metadataResourceProviderResolver = metadataResourceProviderResolver;
        _compatibilityService = compatibilityService;
        _logger = logger;
    }

    public IVersionAwareQueryBuilder ForVersion(FhirVersion version)
    {
        _targetVersion = version;
        _resourceProvider = _metadataResourceProviderResolver.GetProvider(version);
        _logger.LogDebug("Set target FHIR version to {Version}", version);
        return this;
    }

    public IVersionAwareQueryBuilder ForResource(string resourceType)
    {
        if (_resourceProvider == null)
            throw new InvalidOperationException("Must specify FHIR version first using ForVersion()");

        if (!_resourceProvider.IsSupported(resourceType))
            throw new ArgumentException($"Resource type '{resourceType}' is not supported in {_targetVersion}");

        _resourceType = resourceType;
        _logger.LogDebug("Set resource type to {ResourceType}", resourceType);
        return this;
    }

    public IVersionAwareQueryBuilder ForServer(string serverUrl)
    {
        if (string.IsNullOrWhiteSpace(serverUrl))
            throw new ArgumentException("Server URL cannot be null or empty", nameof(serverUrl));

        _serverUrl = serverUrl.TrimEnd('/');
        _logger.LogDebug("Set server URL to {ServerUrl}", _serverUrl);
        return this;
    }

    public IVersionAwareQueryBuilder AddParameter(string name, string value)
        => AddParameter(name, null, value);

    public IVersionAwareQueryBuilder AddParameter(string name, string? modifier, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Parameter name cannot be null or empty", nameof(name));

        _parameters.Add(new QueryParameter { Name = name, Modifier = modifier, Value = value });
        _logger.LogDebug("Added parameter: {Name}{Modifier}={Value}",
            name, string.IsNullOrEmpty(modifier) ? "" : $":{modifier}", value);

        return this;
    }

    public IVersionAwareQueryBuilder WithPaging(int count, int offset = 0)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be greater than 0", nameof(count));
        if (offset < 0)
            throw new ArgumentException("Offset must be non-negative", nameof(offset));

        _count = count;
        _offset = offset;
        _pagingExplicitlySet = true;
        _logger.LogDebug("Set paging: count={Count}, offset={Offset}", count, offset);
        return this;
    }

    public IVersionAwareQueryBuilder OrderBy(string field, bool ascending = true)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Sort field cannot be null or empty", nameof(field));

        _sortParameters.Add(new SortParameter { Field = field, Ascending = ascending });
        _logger.LogDebug("Added sort parameter: {Field} {Direction}", field, ascending ? "ASC" : "DESC");
        return this;
    }

    public IVersionAwareQueryBuilder Include(params string[] includes)
    {
        if (includes?.Length > 0)
        {
            _includes.AddRange(includes);
            _logger.LogDebug("Added includes: {Includes}", string.Join(", ", includes));
        }

        return this;
    }

    public IVersionAwareQueryBuilder RevInclude(params string[] revIncludes)
    {
        if (revIncludes?.Length > 0)
        {
            _revIncludes.AddRange(revIncludes);
            _logger.LogDebug("Added reverse includes: {RevIncludes}", string.Join(", ", revIncludes));
        }

        return this;
    }

    public IVersionAwareQueryBuilder WithCompatibilityCheck(bool enabled = true)
    {
        _compatibilityCheckEnabled = enabled;
        _logger.LogDebug("Compatibility check {Status}", enabled ? "enabled" : "disabled");
        return this;
    }

    public async Task<QueryBuildResult> BuildAsync()
    {
        try
        {
            var validationResult = await ValidateAsync();
            if (!validationResult.IsValid)
            {
                return QueryBuildResult.Failure(
                    $"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.Message))}");
            }

            if (_compatibilityCheckEnabled && !string.IsNullOrEmpty(_serverUrl))
            {
                var compatibilityResult = await CheckServerCompatibilityAsync();
                if (!compatibilityResult.IsSuccess)
                    return compatibilityResult;
            }

            var queryString = BuildQueryString();
            var fullUrl = $"{_serverUrl}/{_resourceType}?{queryString}";

            var result = QueryBuildResult.Success(queryString, fullUrl);
            result.Metadata = new QueryMetadata
            {
                Version = _targetVersion,
                ResourceType = _resourceType,
                ServerUrl = _serverUrl,
                ParameterCount = _parameters.Count,
                EstimatedComplexity = CalculateComplexity(),
                CompatibilityChecked = _compatibilityCheckEnabled
            };

            _logger.LogInformation("Successfully built query for {ResourceType} on {Version}", _resourceType, _targetVersion);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build query");
            return QueryBuildResult.Failure($"Build failed: {ex.Message}");
        }
    }

    public async Task<QueryValidationResult> ValidateAsync()
    {
        var result = new QueryValidationResult();

        if (_resourceProvider == null)
            result.AddError("FHIR version must be specified");

        if (string.IsNullOrEmpty(_resourceType))
            result.AddError("Resource type must be specified");

        if (string.IsNullOrEmpty(_serverUrl))
            result.AddError("Server URL must be specified");

        if (_resourceProvider != null && !string.IsNullOrEmpty(_resourceType))
        {
            if (!_resourceProvider.IsSupported(_resourceType))
                result.AddError($"Resource type '{_resourceType}' is not supported in {_targetVersion}");
            else
                await ValidateSearchParametersAsync(result);
        }

        return await Task.FromResult(result);
    }

    public async Task<List<QueryAlternative>> GetAlternativesAsync()
    {
        var alternatives = new List<QueryAlternative>();

        if (_resourceProvider == null || string.IsNullOrEmpty(_resourceType))
            return alternatives;

        var metadata = _resourceProvider.GetResourceMetadata(_resourceType);
        if (metadata == null)
            return alternatives;

        foreach (var param in _parameters)
        {
            var supportedParam = metadata.SearchParameters
                .FirstOrDefault(sp => sp.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));

            if (supportedParam == null)
            {
                var similarParams = metadata.SearchParameters
                    .Where(sp => sp.Name.Contains(param.Name, StringComparison.OrdinalIgnoreCase) ||
                                 param.Name.Contains(sp.Name, StringComparison.OrdinalIgnoreCase))
                    .Take(3);

                foreach (var similar in similarParams)
                {
                    var alternativeQuery = BuildAlternativeQuery(param, similar);
                    alternatives.Add(new QueryAlternative
                    {
                        Query = alternativeQuery,
                        Description = $"Use '{similar.Name}' instead of '{param.Name}'",
                        CompatibilityScore = CalculateSimilarity(param.Name, similar.Name),
                        Reason = $"Parameter '{param.Name}' not supported, '{similar.Name}' is similar"
                    });
                }
            }
        }

        return await Task.FromResult(alternatives);
    }

    public IVersionAwareQueryBuilder Reset()
    {
        _targetVersion = FhirVersion.R5;
        _resourceType = string.Empty;
        _serverUrl = string.Empty;
        _parameters.Clear();
        _includes.Clear();
        _revIncludes.Clear();
        _sortParameters.Clear();
        _count = 50;
        _offset = 0;
        _pagingExplicitlySet = false;
        _compatibilityCheckEnabled = true;
        _resourceProvider = null;

        _logger.LogDebug("Query builder reset");
        return this;
    }

    private async Task<QueryBuildResult> CheckServerCompatibilityAsync()
    {
        try
        {
            var compatibility = await _compatibilityService.CheckResourceSupportAsync(_serverUrl, _resourceType);

            if (!compatibility.IsSupported)
                return QueryBuildResult.Failure($"Resource '{_resourceType}' is not supported by server");

            var result = QueryBuildResult.Success("", "");

            foreach (var param in _parameters)
            {
                var paramSupport = compatibility.SearchParameters
                    .FirstOrDefault(sp => sp.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));

                if (paramSupport == null)
                    result.Warnings.Add($"Search parameter '{param.Name}' may not be supported by server");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check server compatibility");
            return QueryBuildResult.Success("", "");
        }
    }

    private string BuildQueryString()
    {
        var queryParts = new List<string>();

        foreach (var param in _parameters)
        {
            var paramName = string.IsNullOrEmpty(param.Modifier)
                ? param.Name
                : $"{param.Name}:{param.Modifier}";
            queryParts.Add($"{paramName}={Uri.EscapeDataString(param.Value)}");
        }

        if (_pagingExplicitlySet)
            queryParts.Add($"_count={_count}");

        if (_offset > 0)
            queryParts.Add($"_offset={_offset}");

        if (_sortParameters.Any())
        {
            var sortValues = _sortParameters.Select(sp =>
                sp.Ascending ? sp.Field : $"-{sp.Field}");
            queryParts.Add($"_sort={string.Join(",", sortValues)}");
        }

        foreach (var include in _includes)
            queryParts.Add($"_include={Uri.EscapeDataString(include)}");

        foreach (var revInclude in _revIncludes)
            queryParts.Add($"_revinclude={Uri.EscapeDataString(revInclude)}");

        return string.Join("&", queryParts);
    }

    private async Task ValidateSearchParametersAsync(QueryValidationResult result)
    {
        if (_resourceProvider == null)
            return;

        var metadata = _resourceProvider.GetResourceMetadata(_resourceType);
        if (metadata == null)
            return;

        foreach (var param in _parameters)
        {
            var supportedParam = metadata.SearchParameters
                .FirstOrDefault(sp => sp.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));

            if (supportedParam == null)
            {
                result.AddWarning($"Search parameter '{param.Name}' is not defined for resource '{_resourceType}'");
            }
            else if (!string.IsNullOrEmpty(param.Modifier))
            {
                if (supportedParam.SupportedModifiers.Count > 0 &&
                    !supportedParam.SupportedModifiers.Contains(param.Modifier))
                {
                    result.AddWarning($"Modifier '{param.Modifier}' may not be supported for parameter '{param.Name}'");
                }
            }
        }

        await Task.CompletedTask;
    }

    private int CalculateComplexity()
    {
        var complexity = 1;
        complexity += _parameters.Count;
        complexity += _includes.Count;
        complexity += _revIncludes.Count;
        complexity += _sortParameters.Count;

        return Math.Min(complexity, 10);
    }

    private string BuildAlternativeQuery(QueryParameter original, SearchParameterMetadataEntry alternative)
    {
        var altParams = _parameters.Where(p => p != original).ToList();
        altParams.Add(new QueryParameter
        {
            Name = alternative.Name,
            Modifier = original.Modifier,
            Value = original.Value
        });

        var queryParts = altParams.Select(p =>
        {
            var paramName = string.IsNullOrEmpty(p.Modifier) ? p.Name : $"{p.Name}:{p.Modifier}";
            return $"{paramName}={Uri.EscapeDataString(p.Value)}";
        });

        return string.Join("&", queryParts);
    }

    private static int CalculateSimilarity(string str1, string str2)
    {
        var longer = str1.Length > str2.Length ? str1 : str2;
        var shorter = str1.Length > str2.Length ? str2 : str1;

        if (longer.Length == 0)
            return 100;

        var editDistance = ComputeLevenshteinDistance(longer, shorter);
        return (int)((longer.Length - editDistance) / (double)longer.Length * 100);
    }

    private static int ComputeLevenshteinDistance(string s1, string s2)
    {
        var matrix = new int[s1.Length + 1, s2.Length + 1];

        for (var i = 0; i <= s1.Length; i++)
            matrix[i, 0] = i;

        for (var j = 0; j <= s2.Length; j++)
            matrix[0, j] = j;

        for (var i = 1; i <= s1.Length; i++)
        {
            for (var j = 1; j <= s2.Length; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[s1.Length, s2.Length];
    }
}

/// <summary>查詢參數</summary>
public class QueryParameter
{
    public string Name { get; set; } = string.Empty;
    public string? Modifier { get; set; }
    public string Value { get; set; } = string.Empty;
}

/// <summary>排序參數</summary>
public class SortParameter
{
    public string Field { get; set; } = string.Empty;
    public bool Ascending { get; set; } = true;
}
