using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Fhir.Terminology.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fhir.Terminology.Infrastructure;

/// <summary>
/// HL7 FHIR <strong>Internal Code systems</strong>（<c>terminologies-systems.html</c> §4.3.0.2）：
/// canonical URI 為 <c>http://hl7.org/fhir/{slug}</c>。以 <c>content=not-present</c> 登錄於本機供 SEARCH／術語宣告辨識；
/// 實際碼值仍以 FHIR 規格／套件為準，本機不重複儲存完整概念列。
/// </summary>
public static class InternalFhirCodeSystemsSeed
{
    private const string UrlPrefix = "http://hl7.org/fhir/";
    private const string LogicalIdPrefix = "internal-fhir-";

    /// <summary>依 slug 清單（一行一筆）建立欠缺的 CodeSystem；若同一 <c>url</c> 已存在則略過。</summary>
    public static async Task EnsureRegisteredAsync(
        TerminologyDbContext db,
        ITerminologyRepository repository,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        await using var stream = OpenSlugListStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var inserted = 0;
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            var slug = line.Trim();
            if (slug.Length == 0 || slug.StartsWith("#", StringComparison.Ordinal))
                continue;

            var url = UrlPrefix + slug;
            var urlTaken = await db.TerminologyResources.AsNoTracking()
                .AnyAsync(x => x.ResourceType == "CodeSystem" && x.Url == url, cancellationToken);
            if (urlTaken)
                continue;

            var logicalId = BuildLogicalId(slug);
            var idTaken = await db.TerminologyResources.AsNoTracking()
                .AnyAsync(x => x.ResourceType == "CodeSystem" && x.LogicalId == logicalId, cancellationToken);
            if (idTaken)
            {
                logger.LogWarning(
                    "Skip internal FHIR CodeSystem slug {Slug}: logical id {LogicalId} already used with different URL expectation.",
                    slug, logicalId);
                continue;
            }

            var json = BuildStubJson(logicalId, slug, url);
            await repository.CreateAsync(json, cancellationToken);
            inserted++;
        }

        if (inserted > 0)
            logger.LogInformation("Registered {Inserted} HL7 internal canonical CodeSystem stubs (content=not-present).", inserted);
    }

    private static Stream OpenSlugListStream()
    {
        var asm = typeof(InternalFhirCodeSystemsSeed).Assembly;
        var name = asm.GetManifestResourceNames()
            .SingleOrDefault(static n => n.EndsWith("internal-fhir-code-system-slugs.txt", StringComparison.Ordinal));
        if (name is null)
            throw new InvalidOperationException("Embedded resource internal-fhir-code-system-slugs.txt not found.");

        return asm.GetManifestResourceStream(name)
               ?? throw new InvalidOperationException("Could not open embedded slug list.");
    }

    /// <summary>FHIR id ≤64；過長 slug 改為确定性短 id（SHA256 前 8 bytes hex）。</summary>
    private static string BuildLogicalId(string slug)
    {
        var plain = LogicalIdPrefix + slug;
        if (plain.Length <= 64)
            return plain;

        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(Encoding.UTF8.GetBytes(slug), hash);
        return LogicalIdPrefix + Convert.ToHexString(hash[..16]).ToLowerInvariant();
    }

    private static string BuildStubJson(string logicalId, string slug, string url)
    {
        var name = SlugToName(slug);
        var payload = new Dictionary<string, object?>
        {
            ["resourceType"] = "CodeSystem",
            ["id"] = logicalId,
            ["url"] = url,
            ["version"] = "0",
            ["name"] = name,
            ["title"] = slug,
            ["status"] = "active",
            ["description"] =
                "HL7 FHIR internal code system (terminologies-systems.html §4.3.0.2). Canonical URI " + url +
                ". Codes are defined by the FHIR specification; not duplicated here.",
            ["content"] = "not-present",
        };
        return JsonSerializer.Serialize(payload);
    }

    /// <summary>產生符合 FHIR CodeSystem.name 慣例之簡單 PascalCase（僅供索引／顯示）。</summary>
    private static string SlugToName(string slug)
    {
        var parts = slug.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var sb = new StringBuilder("FHIR");
        foreach (var p in parts)
        {
            if (p.Length == 0) continue;
            sb.Append(char.ToUpperInvariant(p[0]));
            if (p.Length > 1)
                sb.Append(p.AsSpan(1));
        }

        var name = sb.ToString();
        if (name.Length == 0)
            name = "FHIRInternal" + slug.Replace("-", "", StringComparison.Ordinal);
        if (!char.IsLetter(name[0]))
            name = "F" + name;

        return name.Length <= 64 ? name : name[..64];
    }
}
