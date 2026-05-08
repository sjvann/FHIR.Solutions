namespace Fhir.Auth.TokenServer;

/// <summary>OAuth／SMART token 端點回應（access／refresh／patient context）。</summary>
public sealed class SmartTokenResult
{
    public bool Success { get; init; }

    public string? Error { get; init; }

    public string? ErrorDescription { get; init; }

    public string? AccessToken { get; init; }

    public string? RefreshToken { get; init; }

    public string? IdToken { get; init; }

    public string? TokenType { get; init; }

    public int? ExpiresIn { get; init; }

    public string? Scope { get; init; }

    /// <summary>SMART standalone：token 內或回應可帶 patient id。</summary>
    public string? Patient { get; init; }

    public string? RawJson { get; init; }

    public static SmartTokenResult Failed(string error, string? description = null) =>
        new() { Success = false, Error = error, ErrorDescription = description };
}
