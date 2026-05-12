using System.Collections.Concurrent;
using System.Net.Http.Headers;
using Fhir.Auth.TokenServer;
using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Extensions;
using Fhir.QueryBuilder.Metadata;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.QueryBuilder.Models;
using Fhir.VersionManager;
using Fhir.VersionManager.Capability;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Fhir.QueryBuilder.Services;

public sealed class FhirQueryService : IFhirQueryService
{
    private readonly ILogger<FhirQueryService> _logger;
    private readonly QueryBuilderAppSettings _options;
    private readonly FhirQueryBuilderOptions _fhirBuilderOptions;
    private readonly ICacheService _cacheService;
    private readonly IPerformanceService _performanceService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICapabilityContext _capabilityContext;
    private readonly ITokenServer _tokenServer;
    private readonly IFhirCapabilityRuntime _capabilityRuntime;
    private HttpClient? _httpClient;
    private string? _activeBaseUrl;

    /// <summary>component.definition canonical → 所屬 SearchParameter.type（快取，TD-1）。</summary>
    private readonly ConcurrentDictionary<string, string?> _canonicalSearchParameterTypeCache = new();

    public FhirQueryService(
        ILogger<FhirQueryService> logger,
        IOptions<QueryBuilderAppSettings> options,
        FhirQueryBuilderOptions fhirBuilderOptions,
        ICacheService cacheService,
        IPerformanceService performanceService,
        IHttpClientFactory httpClientFactory,
        ICapabilityContext capabilityContext,
        ITokenServer tokenServer,
        IFhirCapabilityRuntime capabilityRuntime)
    {
        _logger = logger;
        _options = options.Value;
        _fhirBuilderOptions = fhirBuilderOptions;
        _cacheService = cacheService;
        _performanceService = performanceService;
        _httpClientFactory = httpClientFactory;
        _capabilityContext = capabilityContext;
        _tokenServer = tokenServer;
        _capabilityRuntime = capabilityRuntime;
    }

    public bool IsConnected => _httpClient != null;
    public string? BaseUrl => _activeBaseUrl;

    /// <summary>SMART well-known 已取得 authorization／token 端點。</summary>
    public bool SupportOAuth => _tokenServer.IsDiscoveryReady;

    public string? AuthorizeUrl => _tokenServer.Configuration?.AuthorizationEndpoint;

    public string? TokenUrl => _tokenServer.Configuration?.TokenEndpoint;

    public async Task<CapabilityParseResult?> ConnectToServerAsync(string baseUrl,
        CancellationToken cancellationToken = default,
        FhirVersion? declaredFhirVersionOverride = null)
    {
        using var perfOperation = _performanceService.StartOperation("ConnectToServer");

        try
        {
            _logger.LogInformation("Connecting to FHIR server: {BaseUrl}", baseUrl);

            var normalized = baseUrl.TrimEnd('/');
            _canonicalSearchParameterTypeCache.Clear();

            var declared = declaredFhirVersionOverride
                           ?? FhirVersionParser.ParseFromShortName(_options.DefaultFhirVersion);
            if (declared == FhirVersion.Unknown)
                declared = _fhirBuilderOptions.DefaultFhirVersion;

            var strategy = _options.FhirVersionResolution;

            var cacheKey = $"capability_{normalized}_d{(int)declared}_s{(int)strategy}";

            if (_options.EnableCaching)
            {
                var cached = await _cacheService.GetAsync<CapabilityParseResult>(cacheKey, cancellationToken);
                if (cached != null)
                {
                    _logger.LogDebug("Using cached capability parse for {BaseUrl}", baseUrl);
                    _httpClient = _httpClientFactory.CreateClient();
                    _httpClient.Timeout = TimeSpan.FromSeconds(_options.RequestTimeoutSeconds);
                    _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/fhir+json");
                    _activeBaseUrl = normalized;
                    _capabilityContext.SetConnection(normalized, cached);
                    await _tokenServer.DiscoverAsync(normalized, cached.Json, cancellationToken).ConfigureAwait(false);
                    perfOperation.AddCustomMetric("CacheHit", true);
                    return cached;
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

            var parse = _capabilityRuntime.ParseMetadata(json, normalized, declared, strategy);
            if (!string.IsNullOrEmpty(parse.MismatchWarning))
                _logger.LogWarning("{Warning}", parse.MismatchWarning);

            _capabilityContext.SetConnection(normalized, parse);

            await _tokenServer.DiscoverAsync(normalized, json, cancellationToken).ConfigureAwait(false);

            if (_options.EnableCaching)
            {
                await _cacheService.SetAsync(cacheKey, parse,
                    TimeSpan.FromMinutes(_options.CacheExpirationMinutes), cancellationToken);
            }

            perfOperation.AddCustomMetric("CacheHit", false);
            perfOperation.AddCustomMetric("ServerUrl", baseUrl);
            _logger.LogInformation("Successfully connected to FHIR server: {BaseUrl}", baseUrl);
            return parse;
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
        var model = _capabilityContext.CapabilityModel;
        var types = model?.ServerResources
            .Select(r => r.Type)
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
        var arr = rc?.SearchIncludes.ToArray();
        return Task.FromResult(arr);
    }

    public Task<string[]?> GetSearchRevIncludeAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        var rc = FindRestResource(resourceName);
        var arr = rc?.SearchRevIncludes.ToArray();
        return Task.FromResult(arr);
    }

    public Task<IEnumerable<SearchParamModel>?> GetSearchParametersAsync(string resourceName,
        CancellationToken cancellationToken = default)
    {
        var rc = FindRestResource(resourceName);
        if (rc == null || rc.SearchParams.Count == 0)
            return Task.FromResult<IEnumerable<SearchParamModel>?>(null);

        var list = new List<SearchParamModel>();
        foreach (var sp in rc.SearchParams)
        {
            var name = sp.Name;
            if (string.IsNullOrEmpty(name))
                continue;

            list.Add(new SearchParamModel
            {
                Name = name,
                Type = sp.Type ?? "",
                Documentation = sp.Documentation
            });
        }

        return Task.FromResult<IEnumerable<SearchParamModel>?>(list);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SearchParamComponentModel>?> TryGetCompositeSearchParameterComponentsAsync(
        string resourceType,
        string searchParamCode,
        CancellationToken cancellationToken = default)
    {
        if (_httpClient == null || string.IsNullOrWhiteSpace(_activeBaseUrl))
            return null;

        var rt = resourceType.Trim();
        var code = searchParamCode.Trim();
        if (rt.Length == 0 || code.Length == 0)
            return null;

        var bundleJson = await FetchCompositeSearchParameterBundleJsonAsync(rt, code, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(bundleJson))
            return null;

        var definitions = SearchParameterCompositeParser.TryGetComponentDefinitionCanonicals(bundleJson);
        if (definitions == null || definitions.Count < 2)
            return null;

        var list = new List<SearchParamComponentModel>();
        for (var i = 0; i < definitions.Count; i++)
        {
            var def = definitions[i];
            var resolvedType = await ResolveCanonicalSearchParameterTypeAsync(def, cancellationToken).ConfigureAwait(false);
            list.Add(new SearchParamComponentModel
            {
                Index = i,
                DefinitionCanonical = def,
                ResolvedParameterType = resolvedType
            });
        }

        return list;
    }

    private async Task<string?> FetchCompositeSearchParameterBundleJsonAsync(string resourceType, string code,
        CancellationToken cancellationToken)
    {
        var baseUrl = _activeBaseUrl!.TrimEnd('/');
        var queries = new[]
        {
            $"{baseUrl}/SearchParameter?base={Uri.EscapeDataString(resourceType)}&code={Uri.EscapeDataString(code)}&_count=8",
            $"{baseUrl}/SearchParameter?code={Uri.EscapeDataString(code)}&_count=8",
        };

        foreach (var url in queries)
        {
            try
            {
                using var resp = await _httpClient!.GetAsync(url, cancellationToken).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode)
                    continue;
                var json = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                if (SearchParameterCompositeParser.TryGetComponentDefinitionCanonicals(json) != null)
                    return json;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "SearchParameter composite probe failed: {Url}", url);
            }
        }

        return null;
    }

    private async Task<string?> ResolveCanonicalSearchParameterTypeAsync(string? canonical,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(canonical) || _httpClient == null)
            return null;

        if (_canonicalSearchParameterTypeCache.TryGetValue(canonical, out var cached))
            return cached;

        var baseUrl = _activeBaseUrl!.TrimEnd('/');
        var lookup =
            $"{baseUrl}/SearchParameter?url={Uri.EscapeDataString(canonical)}&_count=1";

        try
        {
            using var resp = await _httpClient.GetAsync(lookup, cancellationToken).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                _canonicalSearchParameterTypeCache[canonical] = null;
                return null;
            }

            var json = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var type = SearchParameterCompositeParser.TryGetFirstSearchParameterTypeFromBundle(json);
            _canonicalSearchParameterTypeCache[canonical] = type;
            return type;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Resolve SearchParameter type for {Canonical}", canonical);
            _canonicalSearchParameterTypeCache.TryAdd(canonical, null);
            return null;
        }
    }

    public async Task<FhirQueryResult> ExecuteStructuredQueryAsync(string serverUrl, string query,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return new FhirQueryResult { IsSuccess = false, ErrorMessage = "尚未實作", StatusCode = 501 };
    }

    public Task<CapabilityParseResult?> GetCapabilityStatementAsync(string serverUrl,
        CancellationToken cancellationToken = default)
    {
        var normalized = serverUrl.TrimEnd('/');
        if (string.Equals(normalized, _activeBaseUrl, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(_capabilityContext.LastParseResult);

        return Task.FromResult<CapabilityParseResult?>(null);
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
            var json = resp.IsSuccessStatusCode ? await resp.Content.ReadAsStringAsync(cancellationToken) : null;
            sw.Stop();
            FhirVersion? fv = null;
            if (!string.IsNullOrEmpty(json))
            {
                var declared = FhirVersionParser.ParseFromShortName(_options.DefaultFhirVersion);
                if (declared == FhirVersion.Unknown)
                    declared = _fhirBuilderOptions.DefaultFhirVersion;
                fv = _capabilityRuntime.ParseMetadata(json, serverUrl.TrimEnd('/'), declared, _options.FhirVersionResolution)
                    .DetectedVersion;
            }

            return new ServerConnectionResult
            {
                IsConnected = resp.IsSuccessStatusCode,
                ResponseTimeMs = sw.ElapsedMilliseconds,
                StatusCode = (int)resp.StatusCode,
                FhirVersion = fv
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

    public void Disconnect()
    {
        _httpClient?.Dispose();
        _httpClient = null;
        _activeBaseUrl = null;
        _canonicalSearchParameterTypeCache.Clear();
        _capabilityContext.Clear();
    }

    private CapabilityRestResourceModel? FindRestResource(string resourceName)
    {
        var model = _capabilityContext.CapabilityModel;
        return model?.ServerResources.FirstOrDefault(r =>
            string.Equals(r.Type, resourceName, StringComparison.OrdinalIgnoreCase));
    }
}
