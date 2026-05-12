using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Extensions;
using Fhir.QueryBuilder.Services;
using Fhir.QueryBuilder.Services.Interfaces;
using Moq;
using Xunit.Abstractions;

namespace Fhir.QueryBuilder.Tests;

public abstract class TestBase : IDisposable
{
    protected ITestOutputHelper Output { get; }

    protected ServiceProvider ServiceProvider { get; private set; }
    protected IServiceCollection Services { get; private set; }
    protected Mock<ILogger<T>> GetMockLogger<T>() => new Mock<ILogger<T>>();

    protected TestBase(ITestOutputHelper? output = null)
    {
        Output = output ?? NullTestOutput.Instance;
        Services = new ServiceCollection();
        ConfigureServices(Services);
        ServiceProvider = Services.BuildServiceProvider();
    }

    private sealed class NullTestOutput : ITestOutputHelper
    {
        internal static readonly NullTestOutput Instance = new();

        public void WriteLine(string message)
        {
        }

        public void WriteLine(string format, params object[] args)
        {
        }
    }

    protected virtual bool EnableTestCaching => false;

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(GetTestConfiguration())
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<QueryBuilderAppSettings>(configuration.GetSection(QueryBuilderAppSettings.SectionName));

        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

        services.AddFhirQueryBuilderR5(o =>
        {
            o.EnableCaching = EnableTestCaching;
            o.EnablePerformanceMonitoring = false;
            o.EnableCompatibilityCheck = false;
        });

        services.AddSingleton<IPerformanceService, PerformanceService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
    }

    protected virtual Dictionary<string, string?> GetTestConfiguration()
    {
        return new Dictionary<string, string?>
        {
            ["Fhir.QueryBuilder:DefaultServerUrl"] = "https://test.fire.ly",
            ["Fhir.QueryBuilder:DefaultFhirVersion"] = "R5",
            ["Fhir.QueryBuilder:FhirVersionResolution"] = "0",
            ["Fhir.QueryBuilder:RequestTimeoutSeconds"] = "10",
            ["Fhir.QueryBuilder:EnableLogging"] = "true",
            ["Fhir.QueryBuilder:EnableCaching"] = "false",
            ["Fhir.QueryBuilder:CacheExpirationMinutes"] = "5",
            ["Fhir.QueryBuilder:MaxRecentServers"] = "5",
            ["Fhir.QueryBuilder:AutoSaveQueries"] = "false",
            ["Fhir.QueryBuilder:QueryHistoryPath"] = "TestQueryHistory",
            ["Fhir.QueryBuilder:MaxQueryHistoryItems"] = "10",
            ["Fhir.QueryBuilder:Ui:Theme"] = "Light",
            ["Fhir.QueryBuilder:Ui:Language"] = "en-US",
            ["Fhir.QueryBuilder:Performance:MaxConcurrentRequests"] = "2",
            ["Fhir.QueryBuilder:Security:ValidateSslCertificates"] = "false",
            ["Fhir.QueryBuilder:Export:DefaultExportPath"] = "TestExports"
        };
    }

    protected T GetService<T>() where T : notnull
        => ServiceProvider.GetRequiredService<T>();

    protected T? GetOptionalService<T>() where T : class
        => ServiceProvider.GetService<T>();

    public virtual void Dispose()
    {
        ServiceProvider?.Dispose();
        GC.SuppressFinalize(this);
    }
}
