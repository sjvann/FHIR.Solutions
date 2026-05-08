using System.Net;

namespace FhirResourceCreator.Registry;

public sealed class FhirPackageDownloader(HttpClient http)
{
    readonly HttpClient _http = http;

    public async Task<string> DownloadPackageTarballAsync(string registryBaseUrl, string packageId, string version, string destinationDirectory, CancellationToken ct = default)
    {
        Directory.CreateDirectory(destinationDirectory);
        var safeId = packageId.Trim('/');
        var fileName = $"{safeId.Replace('/', '-')}-{version}.tgz";
        var destPath = Path.Combine(destinationDirectory, fileName);

        if (File.Exists(destPath))
            return destPath;

        var baseUrl = registryBaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/{safeId}/{version}";
        using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"Download failed {(int)resp.StatusCode}: {url}");

        await using var src = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        await using var dst = File.Create(destPath);
        await src.CopyToAsync(dst, ct).ConfigureAwait(false);
        return destPath;
    }

    public static HttpClient CreateDefaultClient()
    {
        var h = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        h.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/gzip, application/octet-stream, */*");
        return h;
    }
}
