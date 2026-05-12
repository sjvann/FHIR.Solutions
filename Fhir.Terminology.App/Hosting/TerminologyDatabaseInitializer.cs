using Fhir.Terminology.Core;
using Fhir.Terminology.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhir.Terminology.App.Hosting;

/// <summary>啟動時執行遷移與可選的示範／註冊種子（單一職責，與端點對應分離）。</summary>
internal static class TerminologyDatabaseInitializer
{
    public static async Task RunMigrationsAndSeedAsync(WebApplication app, CancellationToken cancellationToken = default)
    {
        var configuration = app.Configuration;
        var seedDemoResources = configuration.GetValue("Terminology:SeedDemoResources", true);
        var seedExternalCanonical = configuration.GetValue("Terminology:SeedExternalCanonicalCodeSystems", true);
        var seedInternalFhir = configuration.GetValue("Terminology:SeedInternalFhirCodeSystems", true);

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TerminologyDbContext>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var initLogger = loggerFactory.CreateLogger("TerminologyDatabaseInitializer");

        await db.Database.MigrateAsync(cancellationToken);

        var autoSkipStubs = configuration.GetValue("Terminology:AutoSkipCanonicalAndInternalStubsWhenPackageImportExists", false);
        if (autoSkipStubs && await db.TerminologyPackageImports.AsNoTracking().AnyAsync(cancellationToken))
        {
            seedExternalCanonical = false;
            seedInternalFhir = false;
            initLogger.LogInformation(
                "Skipping external canonical and internal FHIR CodeSystem stub seeds because TerminologyPackageImports has rows and Terminology:AutoSkipCanonicalAndInternalStubsWhenPackageImportExists is true.");
        }

        var repo = scope.ServiceProvider.GetRequiredService<ITerminologyRepository>();
        var seedFhirSpecVersion = scope.ServiceProvider
            .GetRequiredService<IOptions<TerminologyServerOptions>>()
            .Value
            .GetEffectiveDefaultImportFhirSpecVersion();
        if (seedDemoResources)
        {
            var seedLogger = loggerFactory.CreateLogger("TerminologyDemoSeed");
            await TerminologyDemoSeed.EnsureDemoResourcesAsync(db, repo, seedFhirSpecVersion, seedLogger, cancellationToken);
        }
        if (seedExternalCanonical)
        {
            var extLogger = loggerFactory.CreateLogger("ExternalCanonicalCodeSystemsSeed");
            await ExternalCanonicalCodeSystemsSeed.EnsureRegisteredAsync(db, repo, seedFhirSpecVersion, extLogger, cancellationToken);
        }
        if (seedInternalFhir)
        {
            var intLogger = loggerFactory.CreateLogger("InternalFhirCodeSystemsSeed");
            await InternalFhirCodeSystemsSeed.EnsureRegisteredAsync(db, repo, seedFhirSpecVersion, intLogger, cancellationToken);
        }
    }
}
