using Fhir.Auth.TokenServer.Configuration;
using Fhir.QueryBuilder.App.Blazor;
using Fhir.QueryBuilder.App.Blazor.Services;
using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Extensions;
using Fhir.QueryBuilder.Platform;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.Services;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.QueryBuilder.ViewModels;
using Fhir.VersionManager;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

try
{
    using var cfgHttp = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    using var stream = await cfgHttp.GetStreamAsync("appsettings.json").ConfigureAwait(false);
    builder.Configuration.AddJsonStream(stream);
}
catch
{
}

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.Configure<QueryBuilderAppSettings>(
    builder.Configuration.GetSection(QueryBuilderAppSettings.SectionName));
builder.Services.Configure<TokenServerOptions>(
    builder.Configuration.GetSection(QueryBuilderAppSettings.SectionName).GetSection("Smart"));

builder.Services.AddSingleton(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

var qbSection = builder.Configuration.GetSection(QueryBuilderAppSettings.SectionName);
var defaultVer = FhirVersionParser.ParseFromShortName(qbSection["DefaultFhirVersion"]);
if (defaultVer == FhirVersion.Unknown)
    defaultVer = FhirVersion.R5;

builder.Services.AddFhirQueryBuilderMultiVersion(
    new[] { FhirVersion.R4, FhirVersion.R4B, FhirVersion.R5 },
    defaultVer);
builder.Services.AddSingleton<Func<IFhirQueryBuilder>>(sp => () => sp.GetRequiredService<IFhirQueryBuilder>());
builder.Services.AddSingleton<IPerformanceService, PerformanceService>();
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
builder.Services.AddSingleton<IConnectionPoolService, ConnectionPoolService>();
builder.Services.AddSingleton<IPaginationService, PaginationService>();
builder.Services.AddSingleton<IQueryTemplateService, QueryTemplateService>();
builder.Services.AddSingleton<IExportService, ExportService>();

builder.Services.AddSingleton<IClipboardTextService, BlazorClipboardTextService>();
builder.Services.AddSingleton<IFilePickerSaveTextService, BlazorFilePickerSaveTextService>();
builder.Services.AddSingleton<IExternalUriLauncher, BlazorExternalUriLauncher>();
builder.Services.AddSingleton<MainViewModel>();

await builder.Build().RunAsync().ConfigureAwait(false);
