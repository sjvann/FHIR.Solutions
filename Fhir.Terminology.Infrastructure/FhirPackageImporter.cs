using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using Fhir.Terminology.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhir.Terminology.Infrastructure;

/// <summary>
/// 自 FHIR NPM 套件（<c>.tgz</c>，內含 <c>package/*.json</c>）匯入 CodeSystem／ValueSet／ConceptMap。
/// 根節點為 <c>Bundle</c> 時匯入各 <c>entry</c> 之 CodeSystem／ValueSet／ConceptMap（HL7 常將 valuesets／conceptmaps 分檔；ConceptMap 多在 conceptmaps 套件檔）；單筆 JSON 亦同。
/// 官方 THO 內容建議使用套件 <c>hl7.terminology</c>；匯入完成後可對照
/// <see cref="ThoCatalogUrls"/> 所列瀏覽頁做抽樣驗證。
/// </summary>
public sealed class FhirPackageImporter(
    TerminologyDbContext db,
    ITerminologyRepository repository,
    IOptions<TerminologyServerOptions> terminologyOptions,
    ILogger<FhirPackageImporter> logger)
{
    private const int DefaultBatchSize = 64;

    private readonly TerminologyDbContext _db = db;
    private readonly ITerminologyRepository _repository = repository;
    private readonly TerminologyServerOptions _terminologyOptions = terminologyOptions.Value;
    private readonly ILogger<FhirPackageImporter> _logger = logger;

    public Task<FhirPackageImportOutcome> ImportFromTgzFileAsync(string tgzPath, CancellationToken cancellationToken = default)
    {
        var full = Path.GetFullPath(tgzPath);
        return ImportFromTgzFileAsync(full, new Uri(full), cancellationToken);
    }

    public async Task<FhirPackageImportOutcome> ImportFromTgzFileAsync(
        string tgzPath,
        Uri sourceUriForAudit,
        CancellationToken cancellationToken = default)
    {
        await using var fs = File.OpenRead(tgzPath);
        var sha = await ComputeSha256HexAsync(fs, cancellationToken);
        fs.Position = 0;
        return await ImportFromTgzStreamAsync(fs, sourceUriForAudit, sha, cancellationToken);
    }

    public async Task<FhirPackageImportOutcome> ImportFromTgzStreamAsync(
        Stream tgzStream,
        Uri? sourceUriForAudit,
        string? sha256HexPrecalculated,
        CancellationToken cancellationToken = default)
    {
        MemoryStream? ownedCopy = null;
        try
        {
            Stream work = tgzStream;
            if (!work.CanSeek)
            {
                ownedCopy = new MemoryStream();
                await tgzStream.CopyToAsync(ownedCopy, cancellationToken);
                ownedCopy.Position = 0;
                work = ownedCopy;
            }

            string? sha = sha256HexPrecalculated;
            if (sha is null)
            {
                sha = await ComputeSha256HexAsync(work, cancellationToken);
                work.Position = 0;
            }

            var manifest = await TryReadPackageManifestAsync(work, cancellationToken);
            work.Position = 0;

            var importFv = FhirSpecVersionNormalizer.Normalize(
                manifest.FhirVersion,
                _terminologyOptions.GetEffectiveDefaultImportFhirSpecVersion());

            await using var gz = new GZipStream(work, CompressionMode.Decompress, leaveOpen: true);
            using var reader = new TarReader(gz, leaveOpen: true);

            string? packageId = manifest.PackageId;
            string? packageVersion = manifest.PackageVersion;
            var batch = new List<string>(DefaultBatchSize);
            var totalImported = 0;
            var skipped = 0;

            while (reader.GetNextEntry(copyData: true) is { } entry)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var name = NormalizeTarPath(entry.Name);
                if (entry.EntryType is not TarEntryType.RegularFile || string.IsNullOrEmpty(name))
                    continue;

                if (!name.StartsWith("package/", StringComparison.Ordinal) || !name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    continue;

                await using var data = entry.DataStream;
                if (data is null)
                    continue;

                var json = await ReadTarEntryAsStringAsync(data, entry, cancellationToken);

                if (string.Equals(name, "package/package.json", StringComparison.OrdinalIgnoreCase))
                {
                    string? pid = packageId, pv = packageVersion, fv = manifest.FhirVersion;
                    ParsePackageManifest(json, ref pid, ref pv, ref fv);
                    packageId = pid;
                    packageVersion = pv;
                    continue;
                }

                var pieces = TerminologyImportJsonExtractor.ExtractImportablePieces(json, out _);
                if (pieces.Count == 0)
                {
                    skipped++;
                    continue;
                }

                foreach (var piece in pieces)
                {
                    batch.Add(piece);
                    if (batch.Count < DefaultBatchSize)
                        continue;

                    totalImported += await _repository.BulkUpsertAsync(batch, importFv, cancellationToken);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
                totalImported += await _repository.BulkUpsertAsync(batch, importFv, cancellationToken);

            var record = new TerminologyPackageImportEntity
            {
                Id = Guid.NewGuid(),
                PackageId = packageId ?? "",
                PackageVersion = packageVersion ?? "",
                FhirVersion = manifest.FhirVersion,
                ImportedAtUtc = DateTimeOffset.UtcNow,
                SourceUri = sourceUriForAudit?.ToString(),
                Sha256Hex = sha,
                ResourceCount = totalImported,
                Notes = skipped > 0 ? $"Skipped non-target JSON entries: {skipped}" : null,
            };

            _db.TerminologyPackageImports.Add(record);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "FHIR package import finished: package={PackageId}@{Version}, fhirSpecVersion={Fv}, resources={Count}, skippedLines={Skipped}, sha256={Sha}",
                record.PackageId,
                record.PackageVersion,
                importFv,
                totalImported,
                skipped,
                sha ?? "(n/a)");

            return new FhirPackageImportOutcome(record.Id, totalImported, skipped, packageId, packageVersion);
        }
        finally
        {
            ownedCopy?.Dispose();
        }
    }

    private sealed record PackageManifest(string? PackageId, string? PackageVersion, string? FhirVersion);

    private static async Task<PackageManifest> TryReadPackageManifestAsync(Stream seekableTgz, CancellationToken cancellationToken)
    {
        await using var gz = new GZipStream(seekableTgz, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new TarReader(gz, leaveOpen: true);

        while (reader.GetNextEntry(copyData: true) is { } entry)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var name = NormalizeTarPath(entry.Name);
            if (entry.EntryType is not TarEntryType.RegularFile || string.IsNullOrEmpty(name))
                continue;

            if (!string.Equals(name, "package/package.json", StringComparison.OrdinalIgnoreCase))
                continue;

            await using var data = entry.DataStream;
            if (data is null)
                return new PackageManifest(null, null, null);

            var json = await ReadTarEntryAsStringAsync(data, entry, cancellationToken);
            string? packageId = null, packageVersion = null, fhirVersion = null;
            ParsePackageManifest(json, ref packageId, ref packageVersion, ref fhirVersion);
            return new PackageManifest(packageId, packageVersion, fhirVersion);
        }

        return new PackageManifest(null, null, null);
    }

    private static async Task<string> ReadTarEntryAsStringAsync(Stream data, TarEntry entry, CancellationToken cancellationToken)
    {
        if (entry.Length > 0)
        {
            using var sr = new StreamReader(data);
            return await sr.ReadToEndAsync(cancellationToken);
        }

        using var ms = new MemoryStream();
        await data.CopyToAsync(ms, cancellationToken);
        return System.Text.Encoding.UTF8.GetString(ms.ToArray());
    }

    private static void ParsePackageManifest(string json, ref string? packageId, ref string? packageVersion, ref string? fhirVersion)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.TryGetProperty("name", out var n))
                packageId = n.GetString();
            if (root.TryGetProperty("version", out var v))
                packageVersion = v.GetString();
            if (root.TryGetProperty("fhirVersion", out var fv))
                fhirVersion = fv.GetString();
            else if (root.TryGetProperty("fhirVersions", out var fvs) && fvs.ValueKind == JsonValueKind.Array && fvs.GetArrayLength() > 0)
                fhirVersion = fvs[0].GetString();
        }
        catch (JsonException)
        {
            /* ignore malformed manifest */
        }
    }

    private static string NormalizeTarPath(string? entryName)
    {
        if (string.IsNullOrEmpty(entryName))
            return "";
        return entryName.Replace('\\', '/').TrimStart('/');
    }

    private static async Task<string> ComputeSha256HexAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var sha = SHA256.Create();
        var hash = await sha.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }
}

public sealed record FhirPackageImportOutcome(Guid RecordId, int ResourcesImported, int SkippedNonTarget, string? PackageId, string? PackageVersion);
