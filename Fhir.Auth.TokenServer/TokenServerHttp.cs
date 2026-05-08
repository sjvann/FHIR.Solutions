namespace Fhir.Auth.TokenServer;

/// <summary>命名 HttpClient（與 <see cref="Microsoft.Extensions.DependencyInjection"/> AddHttpClient 註冊鍵一致）。</summary>
public static class TokenServerHttp
{
    public const string ClientName = "Fhir.Auth.TokenServer.Http";
}
