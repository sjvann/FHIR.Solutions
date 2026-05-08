using System.Net.Http.Headers;
using Fhir.Auth.TokenServer;
using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Metadata;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.QueryBuilder.Models;
using Fhir.Resources.R5;
using Fhir.TypeFramework.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Fhir.QueryBuilder.Services;

public sealed class FhirQueryService : IFhirQueryService
{
    private readonly ILogger<FhirQueryService> _logger;
    private readonly QueryBuilderAppSettings _options;
    private readonly ICacheService _cacheService;
    private readonly IPerformanceService _performanceService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICapabilityContext _capabilityContext;
    private readonly ITokenServer _tokenServer;
    private HttpClient? _httpClient;
    private string? _activeBaseUrl;

    public FhirQueryService(
        ILogger<FhirQueryService> logger,
        IOptions<QueryBuilderAppSettings> options,
        ICacheService cacheService,
        IPerformanceService performanceService,
        IHttpClientFactory httpClientFactory,
        ICapabilityContext capabilityContext,
        ITokenServer tokenServer)
    {
        _logger = logger;
        _options = options.Value;
        _cacheService = cacheService;
        _performanceService = performanceService;
        _httpClientFactory = httpClientFactory;
        _capabilityContext = capabilityContext;
        _tokenServer = tokenServer;
    }

    public bool IsConnected => _httpClient != null;
    public string? BaseUrl => _activeBaseUrl;

    /// <summary>SMART well-known 已取得 authorization／token 端點。</summary>
    public bool SupportOAuth => _tokenServer.IsDiscoveryReady;

    public string? AuthorizeUrl => _tokenServer.Configuration?.AuthorizationEndpoint;

    public string? TokenUrl => _tokenServer.Configuration?.TokenEndpoint;

    public async Task<CapabilityStatement?> ConnectToServerAsync(string baseUrl,
        CancellationToken cancellationToken = default)
    {
        using var perfOperation = _performanceService.StartOperation("ConnectToServer");

        try
        {
            _logger.LogInformation("Connecting to FHIR server: {BaseUrl}", baseUrl);

            var normalized = baseUrl.TrimEnd('/');
            var cacheKey = $"capability_{normalized}";

            if (_options.EnableCaching)
            {
                var cachedCapability = await _cacheService.GetAsync<CapabilityStatement>(cacheKey, cancellationToken);
                if (cachedCapability != null)
                {
                    _logger.LogDebug("Using cached capability statement for {BaseUrl}", baseUrl);
                    _httpClient = _httpClientFactory.CreateClient();
                    _httpClient.Timeout = TimeSpan.FromSeconds(_options.RequestTimeoutSeconds);
                    _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/fhir+json");
                    _activeBaseUrl = normalized;
                    var cachedJson = FhirJsonSerializer.Serialize(cachedCapability);
                    _capabilityContext.SetConnection(normalized, cachedCapability, cachedJson);
                    await _tokenServer.DiscoverAsync(normalized, cachedJson, cancellationToken).ConfigureAwait(false);
                    perfOperation.AddCustomMetric("CacheHit", true);
                    return cachedCapability;
                }
            }

            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.RequestTimeoutSeconds);
            _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/fhir+json");
            _activeBaseUrl = normalized;

            var metadataUrl = $"{normalized}/metadata";
            using var response = await _httpClient.GetAsync(metadataUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var capability = FhirJsonSerializer.Deserialize<CapabilityStatement>(json);
            if (capability == null)
                throw new InvalidOperationException("Failed to deserialize CapabilityStatement from /metadata");

            _capabilityContext.SetConnection(normalized, capability, json);

            await _tokenServer.DiscoverAsync(normalized, json, cancellationToken).ConfigureAwait(false);

            if (_options.EnableCaching)
            {
                await _cacheService.SetAsync(cacheKey, capability,
                    TimeSpan.FromMinutes(_options.CacheExpirationMinutes), cancellationToken);
            }

            perfOperation.AddCustomMetric("CacheHit", false);
            perfOperation.AddCustomMetric("ServerUrl", baseUrl);
            _logger.LogInformation("Successfully connected to FHIR server: {BaseUrl}", baseUrl);
            return capability;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to FHIR server: {BaseUrl}", baseUrl);
            perfOperation.MarkAsFailure(ex.Message);
            _httpClient = null;
            _activeBaseUrl = null;
            throw;
        }
    }

    public async Task<string?> ExecuteQueryAsync(string queryUrl, string? token = null,
        CancellationToken cancellationToken = default)
    {
        if (_httpClient == null || string.IsNullOrEmpty(_activeBaseUrl))
            throw new InvalidOperationException("Not connected to any FHIR server");

        using var perfOperation = _performanceService.StartOperation("ExecuteQuery");

        try
        {
            _logger.LogInformation("Executing FHIR query: {QueryUrl}", queryUrl);

            var bearerForCache = string.IsNullOrEmpty(token) ? _tokenServer.SessionAccessToken : token;
            var cacheKey = $"query_{queryUrl}_{bearerForCache}";

            if (_options.EnableCaching)
            {
                var cachedResult = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);
                if (cachedResult != null)
                {
                    perfOperation.AddCustomMetric("CacheHit", true);
                    perfOperation.AddCustomMetric("QueryUrl", queryUrl);
                    return cachedResult;
                }
            }

            var uri = queryUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? queryUrl
                : $"{_activeBaseUrl}/{queryUrl.TrimStart('/')}";

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.ParseAdd("application/fhir+json");
            var bearer = string.IsNullOrEmpty(token) ? _tokenServer.SessionAccessToken : token;
            if (!string.IsNullOrEmpty(bearer))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync(cancellationToken);

            if (_options.EnableCaching)
            {
                await _cacheService.SetAsync(cacheKey, result,
                    TimeSpan.FromMinutes(_options.CacheExpirationMinutes), cancellationToken);
            }

            perfOperation.AddCustomMetric("CacheHit", false);
            perfOperation.AddCustomMetric("QueryUrl", queryUrl);
            perfOperation.AddCustomMetric("ResultSize", result?.Length ?? 0);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute FHIR query: {QueryUrl}", queryUrl);
            perfOperation.MarkAsFailure(ex.Message);
            throw;
        }
    }

    public Task<IEnumerable<string>?> GetSupportedResourcesAsync(CancellationToken cancellationToken = default)
    {
        var cap = _capabilityContext.Capability;
        var rest = SelectServerRest(cap);
        var types = rest?.Resource?
            .Select(r => (string?)r.Type)
            .Where(s => !string.IsNullOrEmpty(s))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s)
            .ToList();

        if (types is { Count: > 0 })
            return Task.FromResult<IEnumerable<string>?>(types);

        var fallback = CapabilityResourceTypeExtractor.FromCapabilityJson(_capabilityContext.LastCapabilityJson);
        if (fallback is { Count: > 0 })
        {
            _logger.LogDebug("Supported resource types filled from raw CapabilityStatement JSON fallback ({Count} types).",
                fallback.Count);
            return Task.FromResult<IEnumerable<string>?>(fallback);
        }

        return Task.FromResult<IEnumerable<string>?>(types);
    }

    public Task<string[]?> GetSearchIncludeAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        var rc = FindRestResource(resourceName);
        var arr = rc?.SearchInclude?
            .Select(s => (string?)s)
            .Where(s => !string.IsNullOrEmpty(s))
            .Cast<string>()
            .ToArray();
        return Task.FromResult(arr);
    }

    public Task<string[]?> GetSearchRevIncludeAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        var rc = FindRestResource(resourceName);
        var arr = rc?.SearchRevInclude?
            .Select(s => (string?)s)
            .Where(s => !string.IsNullOrEmpty(s))
            .Cast<string>()
            .ToArray();
        return Task.FromResult(arr);
    }

    public Task<IEnumerable<SearchParamModel>?> GetSearchParametersAsync(string resourceName,
        CancellationToken cancellationToken = default)
    {
        var rc = FindRestResource(resourceName);
        if (rc?.SearchParam == null)
            return Task.FromResult<IEnumerable<SearchParamModel>?>(null);

        var list = new List<SearchParamModel>();
        foreach (var sp in rc.SearchParam)
        {
            var name = (string?)sp.Name;
            if (string.IsNullOrEmpty(name))
                continue;

            list.Add(new SearchParamModel
            {
                Name = name,
                Type = (string?)sp.Type ?? "",
                Documentation = (string?)sp.Documentation
            });
        }

        return Task.FromResult<IEnumerable<SearchParamModel>?>(list);
    }

    public async Task<FhirQueryResult> ExecuteStructuredQueryAsync(string serverUrl, string query,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return new FhirQueryResult { IsSuccess = false, ErrorMessage = "尚未實作", StatusCode = 501 };
    }

    public Task<CapabilityStatement?> GetCapabilityStatementAsync(string serverUrl,
        CancellationToken cancellationToken = default)
    {
        var normalized = serverUrl.TrimEnd('/');
        if (string.Equals(normalized, _activeBaseUrl, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(_capabilityContext.Capability);

        return Task.FromResult<CapabilityStatement?>(null);
    }

    public async Task<ServerConnectionResult> TestConnectionAsync(string serverUrl,
        CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_options.RequestTimeoutSeconds);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/fhir+json");
            var url = $"{serverUrl.TrimEnd('/')}/metadata";
            using var resp = await client.GetAsync(url, cancellationToken);
            sw.Stop();
            return new ServerConnectionResult
            {
                IsConnected = resp.IsSuccessStatusCode,
                ResponseTimeMs = sw.ElapsedMilliseconds,
                StatusCode = (int)resp.StatusCode,
                FhirVersion = Common.FhirVersion.R5
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new ServerConnectionResult
            {
                IsConnected = false,
                ResponseTimeMs = sw.ElapsedMilliseconds,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<FhirResourceResult> GetResourceAsync(string serverUrl, string resourceType, string resourceId,
        CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_options.RequestTimeoutSeconds);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/fhir+json");
            var url = $"{serverUrl.TrimEnd('/')}/{resourceType}/{resourceId}";
            using var resp = await client.GetAsync(url, cancellationToken);
            var content = await resp.Content.ReadAsStringAsync(cancellationToken);
            sw.Stop();
            return new FhirResourceResult
            {
                IsSuccess = resp.IsSuccessStatusCode,
                StatusCode = (int)resp.StatusCode,
                Content = content,
                ResourceType = resourceType,
                ResourceId = resourceId,
                ResponseTimeMs = sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new FhirResourceResult
            {
                IsSuccess = false,
                ResourceType = resourceType,
                ResourceId = resourceId,
                ErrorMessage = ex.Message,
                ResponseTimeMs = sw.ElapsedMilliseconds
            };
        }
    }

    private CapabilityStatement.RestComponent.RestResourceComponent? FindRestResource(string resourceName)
    {
        var rest = SelectServerRest(_capabilityContext.Capability);
        return rest?.Resource?.FirstOrDefault(r =>
            string.Equals((string?)r.Type, resourceName, StringComparison.OrdinalIgnoreCase));
    }

    private static CapabilityStatement.RestComponent? SelectServerRest(CapabilityStatement? cap)
    {
        if (cap?.Rest == null || cap.Rest.Count == 0)
            return null;

        var server = cap.Rest.FirstOrDefault(r =>
            string.Equals((string?)r.Mode, "server", StringComparison.OrdinalIgnoreCase));
        if (server?.Resource is { Count: > 0 })
            return server;

        return cap.Rest.FirstOrDefault(r => r.Resource is { Count: > 0 }) ?? server ?? cap.Rest[0];
    }
}
