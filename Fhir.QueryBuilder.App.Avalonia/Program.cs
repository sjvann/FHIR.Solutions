using System;
using Avalonia;
using Fhir.QueryBuilder.App.Avalonia.Services;
using Fhir.Auth.TokenServer.Configuration;
using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Extensions;
using Fhir.QueryBuilder.Localization;
using Fhir.QueryBuilder.Platform;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.Services;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.QueryBuilder.ViewModels;
using Fhir.VersionManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.App.Avalonia;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((ctx, services) =>
            {
                services.Configure<QueryBuilderAppSettings>(
                    ctx.Configuration.GetSection(QueryBuilderAppSettings.SectionName));
                services.Configure<TokenServerOptions>(
                    ctx.Configuration.GetSection(QueryBuilderAppSettings.SectionName).GetSection("Smart"));
                var qbSection = ctx.Configuration.GetSection(QueryBuilderAppSettings.SectionName);
                var defaultVer = FhirVersionParser.ParseFromShortName(qbSection["DefaultFhirVersion"]);
                if (defaultVer == FhirVersion.Unknown)
                    defaultVer = FhirVersion.R5;
                services.AddFhirQueryBuilderMultiVersion(
                    new[] { FhirVersion.R4, FhirVersion.R4B, FhirVersion.R5 },
                    defaultVer);

                services.AddSingleton<Func<IFhirQueryBuilder>>(sp =>
                    () => sp.GetRequiredService<IFhirQueryBuilder>());

                services.AddSingleton<IPerformanceService, PerformanceService>();
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<IConnectionPoolService, ConnectionPoolService>();
                services.AddSingleton<IPaginationService, PaginationService>();
                services.AddSingleton<IQueryTemplateService, QueryTemplateService>();
                services.AddSingleton<IExportService, ExportService>();

                services.AddSingleton<AvaloniaTopLevelAccessor>();
                services.AddSingleton<IClipboardTextService, AvaloniaClipboardTextService>();
                services.AddSingleton<IFilePickerSaveTextService, AvaloniaFilePickerSaveTextService>();
                services.AddSingleton<IApplicationLifetimeService, AvaloniaApplicationLifetimeService>();

                services.AddSingleton<QueryBuilderUiLanguageService>();
                services.AddSingleton<MainViewModel>();
                services.AddTransient<MainWindow>();
            })
            .ConfigureLogging(b =>
            {
                b.AddConsole();
                b.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        AvaloniaUiLanguagePersistence.TryLoad(host.Services.GetRequiredService<QueryBuilderUiLanguageService>());

        var services = host.Services;
        var appBuilder = BuildAvaloniaApp(services);
        appBuilder.StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp(IServiceProvider services)
        => AppBuilder.Configure<App>(() => new App(services))
            .UsePlatformDetect();
}
