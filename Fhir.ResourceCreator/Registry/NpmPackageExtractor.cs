using System.Formats.Tar;
using System.IO.Compression;

namespace FhirResourceCreator.Registry;

public static class NpmPackageExtractor
{
    public static void ExtractTarGz(string tgzPath, string destinationDirectory)
    {
        Directory.CreateDirectory(destinationDirectory);
        using var file = File.OpenRead(tgzPath);
        using var gzip = new GZipStream(file, CompressionMode.Decompress);
        using var reader = new TarReader(gzip);

        while (reader.GetNextEntry() is { } entry)
        {
            if (entry.EntryType is not (TarEntryType.RegularFile or TarEntryType.V7RegularFile))
                continue;

            var name = entry.Name.Replace('\\', '/').TrimStart('.');
            if (string.IsNullOrEmpty(name) || name.Contains("..", StringComparison.Ordinal))
                continue;

            var targetPath = Path.Combine(destinationDirectory, name.Replace('/', Path.DirectorySeparatorChar));
            var dir = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            if (entry.DataStream is null)
                continue;

            using var outFs = File.Create(targetPath);
            entry.DataStream.CopyTo(outFs);
        }
    }
}
