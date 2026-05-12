using Fhir.Terminology.Core;
using Fhir.Terminology.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

if (args.Length < 1)
{
    Console.Error.WriteLine("用法: Fhir.Terminology.PackageImporter <套件.tgz 路徑> [SQLite 連線字串]");
    Console.Error.WriteLine("範例: Fhir.Terminology.PackageImporter hl7.terminology.tgz \"Data Source=terminology.db\"");
    return 1;
}

var tgzPath = Path.GetFullPath(args[0]);
if (!File.Exists(tgzPath))
{
    Console.Error.WriteLine($"找不到檔案: {tgzPath}");
    return 2;
}

var conn = args.Length > 1 ? args[1] : "Data Source=terminology.db";

var services = new ServiceCollection();
services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Information));
services.AddOptions<TerminologyServerOptions>();
services.AddDbContext<TerminologyDbContext>(o => o.UseSqlite(conn));
services.AddScoped<ITerminologyRepository, EfTerminologyRepository>();
services.AddScoped<FhirPackageImporter>();

await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
await scope.ServiceProvider.GetRequiredService<TerminologyDbContext>().Database.MigrateAsync();

var importer = scope.ServiceProvider.GetRequiredService<FhirPackageImporter>();
var uri = new Uri(tgzPath);
var outcome = await importer.ImportFromTgzFileAsync(tgzPath, uri);

Console.WriteLine(
    $"完成：匯入 {outcome.ResourcesImported} 筆資源（略過非目標 JSON：{outcome.SkippedNonTarget}），套件={outcome.PackageId}@{outcome.PackageVersion}，紀錄 Id={outcome.RecordId:D}");
return 0;
