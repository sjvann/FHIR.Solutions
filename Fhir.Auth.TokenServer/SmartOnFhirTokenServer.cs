using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Fhir.Auth.TokenServer.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhir.Auth.TokenServer;

/// <summary>SMART on FHIR／OAuth2 之預設 <see cref="ITokenServer"/> 實作（可於獨立主機託管）。</summary>
public sealed class SmartOnFhirTokenServer : ITokenServer
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IOptions<TokenServerOptions> _options;
    private readonly IOAuthMetadataResolver _metadataResolver;
    private readonly IIdentityTokenPatientExtractor _patientExtractor;
    private readonly ILogger<SmartOnFhirTokenServer> _logger;

    private string? _issuer;
    private SmartConfigurationDocument? _config;
    private string? _accessToken;
    private string? _refreshToken;
    private string? _patientId;

    public SmartOnFhirTokenServer(
        IHttpClientFactory httpFactory,
        IOptions<TokenServerOptions> options,
        IOAuthMetadataResolver metadataResolver,
        IIdentityTokenPatientExtractor patientExtractor,
        ILogger<SmartOnFhirTokenServer> logger)
    {
        _httpFactory = httpFactory;
        _options = options;
        _metadataResolver = metadataResolver;
        _patientExtractor = patientExtractor;
        _logger = logger;
    }

    public SmartConfigurationDocument? Configuration => _config;

    public bool IsDiscoveryReady =>
        !string.IsNullOrEmpty(_config?.AuthorizationEndpoint) &&
        !string.IsNullOrEmpty(_config?.TokenEndpoint);

    public string? SessionAccessToken => _accessToken;

    public string? SessionRefreshToken => _refreshToken;

    public string? SessionPatientId => _patientId;

    public void ClearSession()
    {
        _accessToken = null;
        _refreshToken = null;
        _patientId = null;
    }

    public async Task DiscoverAsync(string fhirBaseUrl, string? capabilityStatementJson = null,
        CancellationToken cancellationToken = default)
    {
        ClearSession();
        _issuer = fhirBaseUrl.TrimEnd('/');
        _config = await _metadataResolver
            .ResolveAsync(_issuer, capabilityStatementJson, cancellationToken)
            .ConfigureAwait(false);
    }

    public SmartAuthorizationStart BeginAuthorizationCodePkce(string? scopesOverride = null,
        string? clientIdOverride = null)
    {
        if (!IsDiscoveryReady || string.IsNullOrEmpty(_issuer))
            throw new InvalidOperationException("SMART discovery not ready.");

        var smart = _options.Value;
        var clientId = string.IsNullOrWhiteSpace(clientIdOverride)
            ? smart.ClientId.Trim()
            : clientIdOverride.Trim();
        if (string.IsNullOrEmpty(clientId))
            throw new InvalidOperationException("Configure TokenServer:ClientId or pass client id.");

        var redirect = smart.RedirectUri.Trim();
        var verifier = PkceHelper.CreateCodeVerifier();
        var challenge = PkceHelper.CreateChallengeS256(verifier);
        var state = PkceHelper.CreateState();
        var scopes = string.IsNullOrWhiteSpace(scopesOverride)
            ? smart.DefaultScopes.Trim()
            : scopesOverride.Trim();

        var authBase = _config!.AuthorizationEndpoint!.Trim();
        var query = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["response_type"] = "code",
            ["client_id"] = clientId,
            ["redirect_uri"] = redirect,
            ["scope"] = scopes,
            ["state"] = state,
            ["code_challenge"] = challenge,
            ["code_challenge_method"] = "S256",
            ["aud"] = _issuer,
        };

        var url = OAuthQueryHelper.AppendQuery(authBase, query);
        return new SmartAuthorizationStart
        {
            AuthorizationUrl = url,
            State = state,
            CodeVerifier = verifier,
            RedirectUri = redirect,
            ClientId = clientId,
        };
    }

    public async Task<SmartTokenResult> ExchangeAuthorizationCodeAsync(Uri callbackUrl, SmartAuthorizationStart start,
        string? clientPrivateKeyPem = null, CancellationToken cancellationToken = default)
    {
        if (_config?.TokenEndpoint == null || string.IsNullOrEmpty(_issuer))
            return SmartTokenResult.Failed("configuration", "Missing token endpoint.");

        var query = OAuthQueryHelper.ParseQuery(callbackUrl.Query);

        if (query.TryGetValue("error", out var oauthErr))
        {
            query.TryGetValue("error_description", out var desc);
            return SmartTokenResult.Failed(oauthErr, desc);
        }

        if (!query.TryGetValue("code", out var code) || string.IsNullOrEmpty(code))
            return SmartTokenResult.Failed("missing_code", "Callback URL has no code.");

        if (!query.TryGetValue("state", out var state) || state != start.State)
            return SmartTokenResult.Failed("invalid_state", "State mismatch (possible CSRF).");

        var form = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = start.RedirectUri,
            ["client_id"] = start.ClientId,
            ["code_verifier"] = start.CodeVerifier,
        };

        AddJwtClientAssertionIfNeeded(form, clientPrivateKeyPem);

        return await PostTokenFormAsync(form, cancellationToken).ConfigureAwait(false);
    }

    public async Task<SmartTokenResult> RefreshAccessTokenAsync(string? clientPrivateKeyPem = null,
        string? clientIdOverride = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_refreshToken))
            return SmartTokenResult.Failed("no_refresh_token", "No refresh token in session.");

        var smart = _options.Value;
        var clientId = string.IsNullOrWhiteSpace(clientIdOverride)
            ? smart.ClientId.Trim()
            : clientIdOverride.Trim();
        if (string.IsNullOrEmpty(clientId))
            return SmartTokenResult.Failed("client_id", "Missing client id.");

        var form = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = _refreshToken!,
            ["client_id"] = clientId,
        };

        AddJwtClientAssertionIfNeeded(form, clientPrivateKeyPem);

        return await PostTokenFormAsync(form, cancellationToken).ConfigureAwait(false);
    }

    public async Task<SmartTokenResult> ClientCredentialsWithJwtAsync(string scope, string privateKeyPem,
        string? clientIdOverride = null, CancellationToken cancellationToken = default)
    {
        if (_config?.TokenEndpoint == null)
            return SmartTokenResult.Failed("configuration", "Missing token endpoint.");

        var smart = _options.Value;
        var clientId = string.IsNullOrWhiteSpace(clientIdOverride)
            ? smart.ClientId.Trim()
            : clientIdOverride.Trim();
        if (string.IsNullOrEmpty(clientId))
            return SmartTokenResult.Failed("client_id", "Configure ClientId.");

        var form = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "client_credentials",
            ["scope"] = scope.Trim(),
            ["client_id"] = clientId,
        };

        AddJwtClientAssertionIfNeeded(form, privateKeyPem);

        return await PostTokenFormAsync(form, cancellationToken).ConfigureAwait(false);
    }

    public async Task<SmartTokenResult> RunAuthorizationCodeFlowWithBrowserAsync(string? scopesOverride = null,
        string? clientPrivateKeyPem = null, string? clientIdOverride = null,
        CancellationToken cancellationToken = default)
    {
        if (!IsDiscoveryReady)
            return SmartTokenResult.Failed("smart_not_ready", "未取得 authorize／token 端點。");

        var start = BeginAuthorizationCodePkce(scopesOverride, clientIdOverride);
        var redirect = _options.Value.RedirectUri.Trim();
        var timeoutSeconds = _options.Value.OAuthLoopbackTimeoutSeconds;
        var timeout = TimeSpan.FromSeconds(Math.Clamp(timeoutSeconds, 10, 600));

        var listenTask = SmartLoopbackListener.WaitForRedirectAsync(redirect, timeout, cancellationToken);
        await Task.Delay(400, cancellationToken).ConfigureAwait(false);

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = start.AuthorizationUrl,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start browser for SMART authorize URL");
            return SmartTokenResult.Failed("browser", ex.Message);
        }

        var callbackUri = await listenTask.ConfigureAwait(false);
        if (callbackUri == null)
            return SmartTokenResult.Failed("redirect_timeout",
                "未收到本機 OAuth redirect。請確認 RedirectUri 已在授權伺服器註冊。");

        return await ExchangeAuthorizationCodeAsync(callbackUri, start, clientPrivateKeyPem, cancellationToken)
            .ConfigureAwait(false);
    }

    private void AddJwtClientAssertionIfNeeded(Dictionary<string, string> form, string? pemOverride)
    {
        var pem = pemOverride ?? _options.Value.ConfidentialPrivateKeyPem;
        if (string.IsNullOrWhiteSpace(pem))
            return;

        if (!form.TryGetValue("client_id", out var cid) || string.IsNullOrEmpty(cid))
            cid = _options.Value.ClientId.Trim();

        var tokenEndpoint = _config!.TokenEndpoint!;
        var jwt = JwtClientAssertionBuilder.CreateAssertionJwt(cid, tokenEndpoint, pem.Trim(),
            TimeSpan.FromMinutes(5));
        form["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
        form["client_assertion"] = jwt;
    }

    private async Task<SmartTokenResult> PostTokenFormAsync(Dictionary<string, string> form,
        CancellationToken cancellationToken)
    {
        var tokenUrl = _config!.TokenEndpoint!;
        using var client = _httpFactory.CreateClient(TokenServerHttp.ClientName);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var content = new FormUrlEncodedContent(form);
        HttpResponseMessage resp;
        try
        {
            resp = await client.PostAsync(tokenUrl, content, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token endpoint POST failed");
            return SmartTokenResult.Failed("http_error", ex.Message);
        }

        var body = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var result = ParseTokenResponse(body);
        result = EnrichPatientFromIdToken(result);

        if (result.Success && !string.IsNullOrEmpty(result.AccessToken))
        {
            _accessToken = result.AccessToken;
            if (!string.IsNullOrEmpty(result.RefreshToken))
                _refreshToken = result.RefreshToken;
            if (!string.IsNullOrEmpty(result.Patient))
                _patientId = result.Patient;
        }

        if (!resp.IsSuccessStatusCode && result.Success)
            return SmartTokenResult.Failed("http_" + (int)resp.StatusCode, body);

        return result;
    }

    private SmartTokenResult EnrichPatientFromIdToken(SmartTokenResult result)
    {
        if (!string.IsNullOrEmpty(result.Patient) || string.IsNullOrEmpty(result.IdToken))
            return result;

        var p = _patientExtractor.TryExtractPatient(result.IdToken);
        if (string.IsNullOrEmpty(p))
            return result;

        return new SmartTokenResult
        {
            Success = result.Success,
            Error = result.Error,
            ErrorDescription = result.ErrorDescription,
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            IdToken = result.IdToken,
            TokenType = result.TokenType,
            ExpiresIn = result.ExpiresIn,
            Scope = result.Scope,
            Patient = p,
            RawJson = result.RawJson,
        };
    }

    private SmartTokenResult ParseTokenResponse(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errEl))
            {
                var code = errEl.GetString() ?? "error";
                var desc = root.TryGetProperty("error_description", out var d)
                    ? d.GetString()
                    : null;
                return SmartTokenResult.Failed(code, desc);
            }

            var access = root.TryGetProperty("access_token", out var at) ? at.GetString() : null;
            var refresh = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;
            var idTok = root.TryGetProperty("id_token", out var idt) ? idt.GetString() : null;
            var tt = root.TryGetProperty("token_type", out var tty) ? tty.GetString() : null;
            var scope = root.TryGetProperty("scope", out var sc) ? sc.GetString() : null;
            var patient = root.TryGetProperty("patient", out var pt) ? pt.GetString() : null;

            int? expires = null;
            if (root.TryGetProperty("expires_in", out var exp) && exp.TryGetInt32(out var sec))
                expires = sec;

            return new SmartTokenResult
            {
                Success = !string.IsNullOrEmpty(access),
                AccessToken = access,
                RefreshToken = refresh,
                IdToken = idTok,
                TokenType = tt,
                Scope = scope,
                Patient = patient,
                ExpiresIn = expires,
                RawJson = json,
                Error = string.IsNullOrEmpty(access) ? "no_access_token" : null,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse token response");
            return SmartTokenResult.Failed("parse_error", ex.Message);
        }
    }
}
