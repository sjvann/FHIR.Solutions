using Microsoft.Extensions.DependencyInjection;

namespace Fhir.Auth.TokenServer.DependencyInjection;

/// <summary>註冊獨立 Token Server 預設實作（OAuth／SMART／JWT assertion／Capability 後援）。</summary>
public static class TokenServerServiceCollectionExtensions
{
    public static IServiceCollection AddFhirAuthTokenServer(this IServiceCollection services)
    {
        services.AddSingleton<IOAuthMetadataResolver, OAuthMetadataResolver>();
        services.AddSingleton<IIdentityTokenPatientExtractor, JwtPayloadPatientExtractor>();
        services.AddSingleton<IEhrLaunchCoordinator, UnsupportedEhrLaunchCoordinator>();
        services.AddSingleton<ITokenBackgroundRefresher, NoOpTokenBackgroundRefresher>();
        services.AddSingleton<ITokenServer, SmartOnFhirTokenServer>();
        return services;
    }

    /// <summary>註冊與 <see cref="SmartOnFhirTokenServer"/> 共用之 HttpClient（逾時可由呼叫端改寫）。</summary>
    public static IServiceCollection AddFhirAuthTokenServerHttpClient(this IServiceCollection services,
        Action<HttpClient>? configureClient = null)
    {
        services.AddHttpClient(TokenServerHttp.ClientName, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(120);
            configureClient?.Invoke(client);
        });
        return services;
    }
}
