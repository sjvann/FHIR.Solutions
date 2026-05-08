using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Fhir.Auth.TokenServer;

/// <summary>先嘗試 <c>.well-known/smart-configuration</c>，缺漏時自 CapabilityStatement extension 補 authorize／token。</summary>
public sealed class OAuthMetadataResolver : IOAuthMetadataResolver
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<OAuthMetadataResolver> _logger;

    public OAuthMetadataResolver(IHttpClientFactory httpFactory, ILogger<OAuthMetadataResolver> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    public async Task<SmartConfigurationDocument?> ResolveAsync(string fhirBaseUrl,
        string? capabilityStatementJson, CancellationToken cancellationToken = default)
    {
        var issuer = fhirBaseUrl.TrimEnd('/');
        var wellKnown = await TryWellKnownAsync(issuer, cancellationToken).ConfigureAwait(false);

        if (HasBothEndpoints(wellKnown))
            return wellKnown;

        if (string.IsNullOrWhiteSpace(capabilityStatementJson))
            return wellKnown;

        var (authCap, tokenCap) = CapabilityStatementOAuthParser.TryExtractEndpoints(capabilityStatementJson);
        if (authCap == null && tokenCap == null)
            return wellKnown;

        _logger.LogInformation("OAuth URIs supplemented from CapabilityStatement extensions.");

        return Merge(wellKnown, authCap, tokenCap);
    }

    private static bool HasBothEndpoints(SmartConfigurationDocument? d) =>
        d != null &&
        !string.IsNullOrEmpty(d.AuthorizationEndpoint) &&
        !string.IsNullOrEmpty(d.TokenEndpoint);

    private static SmartConfigurationDocument Merge(SmartConfigurationDocument? wellKnown,
        string? authorizeFromCap, string? tokenFromCap)
    {
        return new SmartConfigurationDocument
        {
            AuthorizationEndpoint = wellKnown?.AuthorizationEndpoint ?? authorizeFromCap,
            TokenEndpoint = wellKnown?.TokenEndpoint ?? tokenFromCap,
            Issuer = wellKnown?.Issuer,
            JwksUri = wellKnown?.JwksUri,
            Capabilities = wellKnown?.Capabilities,
            ScopesSupported = wellKnown?.ScopesSupported,
        };
    }

    private async Task<SmartConfigurationDocument?> TryWellKnownAsync(string issuer,
        CancellationToken cancellationToken)
    {
        var discoveryUrl = $"{issuer}/.well-known/smart-configuration";

        using var client = _httpFactory.CreateClient(TokenServerHttp.ClientName);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            using var resp = await client.GetAsync(discoveryUrl, cancellationToken).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("well-known SMART discovery HTTP {Status} for {Url}", resp.StatusCode,
                    discoveryUrl);
                return null;
            }

            var json = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<SmartConfigurationDocument>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "well-known SMART discovery failed for {Url}", discoveryUrl);
            return null;
        }
    }
}
