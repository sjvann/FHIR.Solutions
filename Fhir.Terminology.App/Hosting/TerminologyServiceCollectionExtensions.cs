using Fhir.Terminology.Core;
using Fhir.Terminology.Infrastructure;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fhir.Terminology.App.Hosting;

internal static class TerminologyServiceCollectionExtensions
{
    /// <summary>註冊術語服務的選項、永續層、遠端閘道、編排與 OpenAPI 文件產生。</summary>
    public static IServiceCollection AddTerminologyApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var terminologyConn = configuration.GetConnectionString("Terminology") ?? "Data Source=terminology.db";

        services.AddOptions<TerminologyServerOptions>()
            .Configure(o =>
            {
                o.ConnectionString = terminologyConn;
                o.MetadataPublicBaseUri = configuration["Terminology:MetadataPublicBaseUri"] ?? o.MetadataPublicBaseUri;
                o.FhirVersionLabel = configuration["Terminology:FhirVersionLabel"] ?? o.FhirVersionLabel;
                o.DefaultImportFhirSpecVersion = configuration["Terminology:DefaultImportFhirSpecVersion"] ?? o.DefaultImportFhirSpecVersion;
            })
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "SQLite connection string must not be empty")
            .Validate(o => !string.IsNullOrWhiteSpace(o.MetadataPublicBaseUri), "Terminology:MetadataPublicBaseUri must not be empty")
            .ValidateOnStart();

        services.AddDbContext<TerminologyDbContext>(o => o.UseSqlite(terminologyConn));
        services.AddScoped<ITerminologyRepository, EfTerminologyRepository>();
        services.AddScoped<FhirPackageImporter>();
        services.AddHttpClient(RemoteTerminologyGateway.ClientName, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(120);
        });
        services.AddScoped<IRemoteTerminologyGateway, RemoteTerminologyGateway>();
        services.AddScoped<TerminologyOrchestrator>();
        services.AddRazorComponents().AddInteractiveServerComponents();

        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer(TerminologyOpenApiDocumentTransformer.ConfigureDocumentAsync);
        });

        return services;
    }
}
