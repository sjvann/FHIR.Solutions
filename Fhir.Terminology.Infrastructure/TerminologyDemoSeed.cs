using Fhir.Terminology.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fhir.Terminology.Infrastructure;

/// <summary>
/// 於<strong>空資料庫</strong>寫入最小示範資源，方便首次啟動即可試 <c>$expand</c>／<c>$lookup</c>。
/// 是否執行由組態 <c>Terminology:SeedDemoResources</c> 控制；若資料庫已有任一術語資源則略過。
/// </summary>
public static class TerminologyDemoSeed
{
    /// <summary>HL7 administrative-gender（精簡概念列）。</summary>
    public const string DemoCodeSystemJson =
        """
        {"resourceType":"CodeSystem","id":"demo-cs-administrative-gender","url":"http://hl7.org/fhir/administrative-gender","version":"4.0.1","status":"active","content":"complete","concept":[
          {"code":"male","display":"Male"},
          {"code":"female","display":"Female"},
          {"code":"other","display":"Other"},
          {"code":"unknown","display":"Unknown"}
        ]}
        """;

    /// <summary>引用上述 CodeSystem 全碼的 ValueSet。</summary>
    public const string DemoValueSetJson =
        """
        {"resourceType":"ValueSet","id":"demo-vs-administrative-gender","url":"http://example.org/fhir/terminology/demo/administrative-gender-all","name":"demo-administrative-gender-all","title":"示範：行政性別（全部碼）","status":"active","compose":{"include":[{"system":"http://hl7.org/fhir/administrative-gender"}]}}
        """;

    private static readonly string[] DemoResourcesInOrder = [DemoCodeSystemJson, DemoValueSetJson];

    public static async Task EnsureDemoResourcesAsync(
        TerminologyDbContext db,
        ITerminologyRepository repository,
        string seedFhirSpecVersion,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (await db.TerminologyResources.AsNoTracking().AnyAsync(cancellationToken))
            return;

        foreach (var json in DemoResourcesInOrder)
            await repository.CreateAsync(json, seedFhirSpecVersion, cancellationToken);

        logger.LogInformation("Seeded {Count} demo terminology resources (database was empty).", DemoResourcesInOrder.Length);
    }
}
