using Fhir.Terminology.Core;
using Fhir.Terminology.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Fhir.Terminology.Tests;

public class FhirPackageImporterTests
{
    [Fact]
    public async Task ImportFromTgz_inserts_code_system_and_records_package_row()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), "terminology-pkg-test-" + Guid.NewGuid().ToString("n") + ".db");
        try
        {
            var conn = $"Data Source={dbPath}";
            var options = new DbContextOptionsBuilder<TerminologyDbContext>().UseSqlite(conn).Options;
            await using (var init = new TerminologyDbContext(options))
                await init.Database.MigrateAsync();

            var tgzPath = Path.Combine(AppContext.BaseDirectory, "TestData", "minimal-fhir-package.tgz");
            Assert.True(File.Exists(tgzPath), $"Missing fixture: {tgzPath}");

            await using var tgzStream = File.OpenRead(tgzPath);
            await using var db = new TerminologyDbContext(options);
            var repo = new EfTerminologyRepository(db, NullLogger<EfTerminologyRepository>.Instance);
            var opts = Options.Create(new TerminologyServerOptions { FhirVersionLabel = "5.0.0" });
            var importer = new FhirPackageImporter(db, repo, opts, NullLogger<FhirPackageImporter>.Instance);

            var outcome = await importer.ImportFromTgzStreamAsync(tgzStream, new Uri("urn:test:fixture"), sha256HexPrecalculated: null);

            Assert.True(outcome.ResourcesImported >= 1);
            Assert.NotNull(await db.TerminologyPackageImports.AsNoTracking().FirstOrDefaultAsync());
            var row = await db.TerminologyResources.AsNoTracking()
                .SingleAsync(x => x.ResourceType == "CodeSystem" && x.LogicalId == "fixture-import-cs");
            Assert.Contains("http://example.org/cs/import-test", row.Url, StringComparison.Ordinal);
            Assert.Equal("5.0.0", row.FhirSpecVersion);
        }
        finally
        {
            TryDelete(dbPath);
        }
    }

    [Fact]
    public async Task ImportFromTgz_expands_bundle_and_inserts_each_valueset_row()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), "terminology-pkg-bundle-test-" + Guid.NewGuid().ToString("n") + ".db");
        try
        {
            var conn = $"Data Source={dbPath}";
            var options = new DbContextOptionsBuilder<TerminologyDbContext>().UseSqlite(conn).Options;
            await using (var init = new TerminologyDbContext(options))
                await init.Database.MigrateAsync();

            var tgzPath = Path.Combine(AppContext.BaseDirectory, "TestData", "bundle-two-valuesets.tgz");
            Assert.True(File.Exists(tgzPath), $"Missing fixture: {tgzPath}");

            await using var tgzStream = File.OpenRead(tgzPath);
            await using var db = new TerminologyDbContext(options);
            var repo = new EfTerminologyRepository(db, NullLogger<EfTerminologyRepository>.Instance);
            var opts = Options.Create(new TerminologyServerOptions { FhirVersionLabel = "5.0.0" });
            var importer = new FhirPackageImporter(db, repo, opts, NullLogger<FhirPackageImporter>.Instance);

            var outcome = await importer.ImportFromTgzStreamAsync(tgzStream, new Uri("urn:test:bundle"), sha256HexPrecalculated: null);

            Assert.True(outcome.ResourcesImported >= 2, $"expected at least 2 resources, got {outcome.ResourcesImported}");
            var vsIds = await db.TerminologyResources.AsNoTracking()
                .Where(x => x.ResourceType == "ValueSet"
                    && (x.LogicalId == "fixture-import-vs-a" || x.LogicalId == "fixture-import-vs-b"))
                .Select(x => x.LogicalId)
                .ToListAsync();
            Assert.Equal(2, vsIds.Count);
            Assert.Contains("fixture-import-vs-a", vsIds);
            Assert.Contains("fixture-import-vs-b", vsIds);
        }
        finally
        {
            TryDelete(dbPath);
        }
    }

    private static void TryDelete(string dbPath)
    {
        try
        {
            if (File.Exists(dbPath))
                File.Delete(dbPath);
            foreach (var suffix in new[] { "-wal", "-shm" })
            {
                var p = dbPath + suffix;
                if (File.Exists(p))
                    File.Delete(p);
            }
        }
        catch
        {
            /* temp cleanup best-effort */
        }
    }
}
