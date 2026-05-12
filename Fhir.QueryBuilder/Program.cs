using Fhir.Auth.TokenServer.Configuration;
using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Extensions;
using Fhir.QueryBuilder.Platform;
using Fhir.QueryBuilder.PlatformServices;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.Services;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.QueryBuilder.ViewModels;
using Fhir.VersionManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Fhir.QueryBuilder
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 如果有命令列參數 "test"，執行測試
            if (args.Length > 0 && args[0] == "test")
            {
                TestAdvancedFeatures.Run(args);
                return;
            }

            ApplicationConfiguration.Initialize();

            var host = CreateHostBuilder().Build();

            using var scope = host.Services.CreateScope();
            var mainForm = scope.ServiceProvider.GetRequiredService<NewMainForm>();
            Application.Run(mainForm);
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddFhirEnvironmentVariables();
                    config.AddCommandLine(Environment.GetCommandLineArgs());
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<QueryBuilderAppSettings>(
                        context.Configuration.GetSection(QueryBuilderAppSettings.SectionName));
                    services.Configure<TokenServerOptions>(
                        context.Configuration.GetSection(QueryBuilderAppSettings.SectionName).GetSection("Smart"));

                    var qbSection = context.Configuration.GetSection(QueryBuilderAppSettings.SectionName);
                    var defaultVer = FhirVersionParser.ParseFromShortName(qbSection["DefaultFhirVersion"]);
                    if (defaultVer == FhirVersion.Unknown)
                        defaultVer = FhirVersion.R5;
                    services.AddFhirQueryBuilderMultiVersion(
                        new[] { FhirVersion.R4, FhirVersion.R4B, FhirVersion.R5 },
                        defaultVer);

                    services.AddSingleton<Func<IFhirQueryBuilder>>(sp =>
                        () => sp.GetRequiredService<IFhirQueryBuilder>());

                    services.AddSingleton<IConfigurationService, ConfigurationService>();
                    services.AddSingleton<IPerformanceService, PerformanceService>();
                    services.AddSingleton<IConnectionPoolService, ConnectionPoolService>();
                    services.AddSingleton<IPaginationService, PaginationService>();
                    services.AddSingleton<IQueryTemplateService, QueryTemplateService>();
                    services.AddSingleton<IExportService, ExportService>();

                    services.AddSingleton<IClipboardTextService, WindowsClipboardTextService>();
                    services.AddSingleton<IFilePickerSaveTextService, WindowsFilePickerSaveTextService>();

                    services.AddTransient<MainViewModel>();
                    services.AddTransient<NewMainForm>();

                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.AddDebug();
                    });
                })
                .UseConsoleLifetime();
        }
    }
}