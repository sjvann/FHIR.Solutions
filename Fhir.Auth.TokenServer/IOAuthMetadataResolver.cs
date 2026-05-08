namespace Fhir.Auth.TokenServer;

/// <summary>合併 well-known 與 CapabilityStatement 後援之 OAuth 端點解析（由 Token Server 實作，不在 Query Builder）。</summary>
public interface IOAuthMetadataResolver
{
    /// <summary>
    /// <paramref name="capabilityStatementJson"/> 為 <c>/metadata</c> 原始 JSON（可為 null）。
    /// </summary>
    Task<SmartConfigurationDocument?> ResolveAsync(string fhirBaseUrl, string? capabilityStatementJson,
        CancellationToken cancellationToken = default);
}
