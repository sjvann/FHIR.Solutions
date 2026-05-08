namespace Fhir.Auth.TokenServer;

/// <summary>PKCE 授權碼流程啟動後，瀏覽器應開啟的網址與伺服器回呼驗證用 state。</summary>
public sealed class SmartAuthorizationStart
{
    public required string AuthorizationUrl { get; init; }

    public required string State { get; init; }

    public required string CodeVerifier { get; init; }

    public required string RedirectUri { get; init; }

    /// <summary>建立授權 URL 時使用的 client_id（可為 UI 覆寫值）。</summary>
    public required string ClientId { get; init; }
}
