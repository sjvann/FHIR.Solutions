using System.ComponentModel.DataAnnotations;

namespace Fhir.Auth.TokenServer.Configuration;

/// <summary>綁定 appsettings <c>Fhir.QueryBuilder:Smart</c>（或獨立 Token Server 設定區段）。</summary>
public sealed class TokenServerOptions
{
    /// <summary>在 IdP／授權伺服器註冊的 client_id。</summary>
    public string ClientId { get; set; } = "";

    /// <summary>必須與授權伺服器註冊完全一致（預設本機 loopback）。</summary>
    public string RedirectUri { get; set; } = "http://127.0.0.1:9876/callback";

    public string DefaultScopes { get; set; } =
        "launch/patient patient/*.read openid fhirUser offline_access";

    /// <summary>機密客戶端：RSA/EC private key PEM。</summary>
    public string? ConfidentialPrivateKeyPem { get; set; }

    [Range(10, 600)]
    public int OAuthLoopbackTimeoutSeconds { get; set; } = 120;
}
