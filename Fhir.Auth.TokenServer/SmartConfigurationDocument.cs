using System.Text.Json.Serialization;

namespace Fhir.Auth.TokenServer;

/// <summary>SMART App Launch：<c>/.well-known/smart-configuration</c> JSON（精簡常用欄位）。</summary>
public sealed class SmartConfigurationDocument
{
    [JsonPropertyName("authorization_endpoint")]
    public string? AuthorizationEndpoint { get; init; }

    [JsonPropertyName("token_endpoint")]
    public string? TokenEndpoint { get; init; }

    [JsonPropertyName("issuer")]
    public string? Issuer { get; init; }

    [JsonPropertyName("jwks_uri")]
    public string? JwksUri { get; init; }

    [JsonPropertyName("capabilities")]
    public List<string>? Capabilities { get; init; }

    [JsonPropertyName("scopes_supported")]
    public List<string>? ScopesSupported { get; init; }
}
