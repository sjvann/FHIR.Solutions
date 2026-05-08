using System.Text.Json;

namespace FhirResourceCreator.Pipeline;

/// <summary>
/// Scans an extracted NPM FHIR package folder for JSON instances and groups file paths by top-level <c>resourceType</c>.
/// </summary>
public static class FhirPackageExampleHarvester
{
    const int MaxExamplesPerResource = 5;

    /// <summary>
    /// Build resourceType → ordered list of JSON file paths (package-relative examples / illustrations).
    /// </summary>
    public static Dictionary<string, List<string>> BuildResourceInstanceIndex(string packageDir)
    {
        var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        if (!Directory.Exists(packageDir))
            return map;

        foreach (var file in Directory.EnumerateFiles(packageDir, "*.json", SearchOption.AllDirectories)
                     .OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            var fn = Path.GetFileName(file);
            if (string.Equals(fn, "package.json", StringComparison.OrdinalIgnoreCase))
                continue;
            if (string.Equals(fn, ".index.json", StringComparison.OrdinalIgnoreCase))
                continue;

            string? resourceType;
            try
            {
                resourceType = TryReadResourceType(file);
            }
            catch
            {
                continue;
            }

            if (string.IsNullOrEmpty(resourceType))
                continue;

            if (!map.TryGetValue(resourceType, out var list))
            {
                list = new List<string>();
                map[resourceType] = list;
            }

            if (list.Count >= MaxExamplesPerResource)
                continue;

            list.Add(file);
        }

        return map;
    }

    static string? TryReadResourceType(string jsonPath)
    {
        var text = File.ReadAllText(jsonPath);
        using var doc = JsonDocument.Parse(text);
        if (!doc.RootElement.TryGetProperty("resourceType", out var rt))
            return null;
        if (rt.ValueKind != JsonValueKind.String)
            return null;
        return rt.GetString();
    }

    /// <summary>
    /// Copy up to <see cref="MaxExamplesPerResource"/> example JSON files into <paramref name="testDataDir"/>.
    /// Returns relative paths (under TestData) suitable for test assertions and MemberData.
    /// </summary>
    public static IReadOnlyList<string> CopyExamplesToTestData(
        string resourceTypeName,
        IReadOnlyDictionary<string, List<string>> index,
        string testDataDir)
    {
        Directory.CreateDirectory(testDataDir);
        var relativeNames = new List<string>();

        if (index.TryGetValue(resourceTypeName, out var sources) && sources.Count > 0)
        {
            for (var i = 0; i < sources.Count; i++)
            {
                var src = sources[i];
                var destName = i == 0
                    ? $"{resourceTypeName}.example.json"
                    : $"{resourceTypeName}.example.{i + 1}.json";
                var destPath = Path.Combine(testDataDir, destName);
                try
                {
                    File.Copy(src, destPath, overwrite: true);
                    relativeNames.Add(destName);
                }
                catch
                {
                    // ignore unreadable copies
                }
            }
        }

        if (relativeNames.Count == 0)
        {
            var fallbackName = $"{resourceTypeName}.example.json";
            var fallbackPath = Path.Combine(testDataDir, fallbackName);
            var minimal = $$"""{"resourceType":"{{resourceTypeName}}","id":"minimal-example"}""";
            File.WriteAllText(fallbackPath, minimal);
            relativeNames.Add(fallbackName);
        }

        return relativeNames;
    }
}
