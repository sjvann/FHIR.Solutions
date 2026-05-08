namespace Fhir.Auth.TokenServer;

/// <summary>OAuth／SMART token 邊界（取代 Query Builder 內嵌之授權服務）。</summary>
public interface ITokenServer
{
    SmartConfigurationDocument? Configuration { get; }

    bool IsDiscoveryReady { get; }

    string? SessionAccessToken { get; }

    string? SessionRefreshToken { get; }

    string? SessionPatientId { get; }

    /// <summary>擷取端點：well-known + 選填 CapabilityStatement JSON 後援。</summary>
    Task DiscoverAsync(string fhirBaseUrl, string? capabilityStatementJson = null,
        CancellationToken cancellationToken = default);

    SmartAuthorizationStart BeginAuthorizationCodePkce(string? scopesOverride = null,
        string? clientIdOverride = null);

    Task<SmartTokenResult> ExchangeAuthorizationCodeAsync(Uri callbackUrl, SmartAuthorizationStart start,
        string? clientPrivateKeyPem = null, CancellationToken cancellationToken = default);

    Task<SmartTokenResult> RefreshAccessTokenAsync(string? clientPrivateKeyPem = null,
        string? clientIdOverride = null, CancellationToken cancellationToken = default);

    Task<SmartTokenResult> ClientCredentialsWithJwtAsync(string scope, string privateKeyPem,
        string? clientIdOverride = null, CancellationToken cancellationToken = default);

    Task<SmartTokenResult> RunAuthorizationCodeFlowWithBrowserAsync(string? scopesOverride = null,
        string? clientPrivateKeyPem = null, string? clientIdOverride = null,
        CancellationToken cancellationToken = default);

    void ClearSession();
}
