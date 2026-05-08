using Fhir.QueryBuilder.Common;
using Fhir.QueryBuilder.Metadata;
using Fhir.QueryBuilder.QueryBuilders;
using Fhir.QueryBuilder.QueryBuilders.Advanced;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Fhir.QueryBuilder.QueryCore;
using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Services;
using Fhir.Auth.TokenServer.DependencyInjection;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Fhir.QueryBuilder.Extensions;

/// <summary>FHIR Query Builder 服務註冊擴展</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFhirQueryBuilder(
        this IServiceCollection services,
        Action<FhirQueryBuilderOptions>? configure = null)
    {
        var options = new FhirQueryBuilderOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        RegisterCacheServices(services, options);
        RegisterCoreServices(services);
        RegisterFluentApiServices(services);
        RegisterQueryBuilderServices(services, options);
        RegisterCompatibilityServices(services, options);
        RegisterHttpServices(services, options);

        return services;
    }

    public static IServiceCollection AddFhirQueryBuilderR5(
        this IServiceCollection services,
        Action<FhirQueryBuilderOptions>? configure = null)
    {
        return services.AddFhirQueryBuilder(options =>
        {
            options.DefaultFhirVersion = FhirVersion.R5;
            options.SupportedVersions.Add(FhirVersion.R5);
            configure?.Invoke(options);
        });
    }

    public static IServiceCollection AddFhirQueryBuilderMultiVersion(
        this IServiceCollection services,
        IEnumerable<FhirVersion> versions,
        FhirVersion defaultVersion = FhirVersion.R5,
        Action<FhirQueryBuilderOptions>? configure = null)
    {
        return services.AddFhirQueryBuilder(options =>
        {
            options.DefaultFhirVersion = defaultVersion;
            options.SupportedVersions.AddRange(versions);
            configure?.Invoke(options);
        });
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddSingleton<ICapabilityContext, CapabilityContext>();
        services.AddSingleton<IMetadataResourceProvider, R5CapabilityMetadataProvider>();
        services.AddSingleton<IMetadataResourceProviderResolver>(sp =>
        {
            var provider = sp.GetRequiredService<IMetadataResourceProvider>();
            return new MetadataResourceProviderResolver(new[] { provider });
        });
        services.AddSingleton<ISearchParameterRegistry, PermissiveSearchParameterRegistry>();
    }

    private static void RegisterFluentApiServices(IServiceCollection services)
    {
        services.AddSingleton<IValidationService, ValidationService>();
        services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
        services.AddSingleton<IProgressService, ProgressService>();

        services.AddTransient<ISearchParameterBuilder, StringParameterBuilder>();
        services.AddTransient<ISearchParameterBuilder, DateParameterBuilder>();
        services.AddTransient<ISearchParameterBuilder, NumberParameterBuilder>();
        services.AddTransient<ISearchParameterBuilder, TokenParameterBuilder>();
        services.AddTransient<ISearchParameterBuilder, ReferenceParameterBuilder>();
        services.AddTransient<ISearchParameterBuilder, QuantityParameterBuilder>();
        services.AddTransient<ISearchParameterBuilder, UriParameterBuilder>();
        services.AddTransient<ISearchParameterBuilder, CompositeParameterBuilder>();
        services.AddTransient<ISearchParameterBuilder, FilterParameterBuilder>();
        services.AddSingleton<ISearchParameterFactory, SearchParameterFactory>();

        services.AddTransient<IFhirQueryBuilder, FhirQueryBuilder>();
    }

    private static void RegisterCacheServices(IServiceCollection services, FhirQueryBuilderOptions options)
    {
        if (options.EnableCaching)
        {
            switch (options.CacheProvider)
            {
                case CacheProvider.Memory:
                    services.AddSingleton<ICacheService, MemoryCacheService>();
                    break;
                case CacheProvider.Redis:
                    services.AddSingleton<ICacheService, MemoryCacheService>();
                    break;
                default:
                    services.AddSingleton<ICacheService, MemoryCacheService>();
                    break;
            }
        }
        else
            services.AddSingleton<ICacheService, NullCacheService>();
    }

    private static void RegisterQueryBuilderServices(IServiceCollection services, FhirQueryBuilderOptions options)
    {
        services.AddTransient<IVersionAwareQueryBuilder, VersionAwareQueryBuilder>();
        services.AddFhirAuthTokenServer();
        services.AddSingleton<IFhirQueryService, FhirQueryService>();

        if (options.EnablePerformanceMonitoring)
            services.AddSingleton<IPerformanceMonitoringService, PerformanceMonitoringService>();
    }

    private static void RegisterCompatibilityServices(IServiceCollection services, FhirQueryBuilderOptions options)
    {
        services.AddSingleton<IServerCompatibilityService, ServerCompatibilityService>();
        services.AddSingleton<ICapabilityAnalyzer, CapabilityAnalyzer>();
    }

    private static void RegisterHttpServices(IServiceCollection services, FhirQueryBuilderOptions options)
    {
        services.AddHttpClient();
        services.AddFhirAuthTokenServerHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(options.HttpTimeoutSeconds);
        });
        services.AddHttpClient<IServerCompatibilityService, ServerCompatibilityService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(options.HttpTimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/fhir+json");
            client.DefaultRequestHeaders.Add("User-Agent", "FhirQueryBuilder/1.0");
        });
    }
}

/// <summary>DI 用的 Query Builder 選項（與 appsettings 之 <see cref="Configuration.QueryBuilderAppSettings"/> 分離）。</summary>
public class FhirQueryBuilderOptions
{
    public FhirVersion DefaultFhirVersion { get; set; } = FhirVersion.R5;

    public List<FhirVersion> SupportedVersions { get; set; } = new();

    public bool EnableValidation { get; set; } = true;

    public bool EnableCaching { get; set; } = true;

    public CacheProvider CacheProvider { get; set; } = CacheProvider.Memory;

    public int CacheExpirationHours { get; set; } = 24;

    public bool EnablePerformanceMonitoring { get; set; } = false;

    public int HttpTimeoutSeconds { get; set; } = 30;

    public bool EnableCompatibilityCheck { get; set; } = true;

    public QueryBuilderOptions QueryBuilder { get; set; } = new();

    public LoggingOptions Logging { get; set; } = new();
}

public enum CacheProvider
{
    Memory,
    Redis,
    None
}

public class QueryBuilderOptions
{
    public int DefaultPageSize { get; set; } = 50;

    public int MaxPageSize { get; set; } = 1000;

    public bool AutoEncodeParameterValues { get; set; } = true;

    public bool ValidateSearchParameters { get; set; } = true;

    public int MaxQueryComplexity { get; set; } = 10;
}

public class LoggingOptions
{
    public bool LogQueryBuilding { get; set; } = true;

    public bool LogCompatibilityChecks { get; set; } = true;

    public bool LogCacheOperations { get; set; } = false;

    public bool LogPerformanceMetrics { get; set; } = false;
}

public class NullCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        => Task.FromResult<T?>(null);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        => Task.CompletedTask;

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task ClearAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new CacheStatistics());
}
