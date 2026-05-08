using System.Text.Json;

namespace Fhir.Terminology.Core;

/// <summary>從 CodeSystem JSON 遞迴擷取 concept，並計算 subsumes。</summary>
public static class CodeSystemCodeIndex
{
    public static IReadOnlyList<(string Code, string? Display)> ListCodes(string codeSystemJson)
    {
        using var doc = JsonDocument.Parse(codeSystemJson);
        if (doc.RootElement.TryGetProperty("concept", out var rootConcepts) && rootConcepts.ValueKind == JsonValueKind.Array)
        {
            var list = new List<(string, string?)>();
            foreach (var c in rootConcepts.EnumerateArray())
                WalkCollect(c, list);
            return list;
        }

        return Array.Empty<(string, string?)>();
    }

    private static void WalkCollect(JsonElement node, List<(string Code, string? Display)> acc)
    {
        var code = node.TryGetProperty("code", out var c) ? c.GetString() : null;
        if (code is not null)
        {
            var disp = node.TryGetProperty("display", out var d) ? d.GetString() : null;
            acc.Add((code, disp));
        }

        if (node.TryGetProperty("concept", out var children) && children.ValueKind == JsonValueKind.Array)
        {
            foreach (var ch in children.EnumerateArray())
                WalkCollect(ch, acc);
        }
    }

    /// <summary>自根到目標 code 之路徑（含目標）；找不到則 null。</summary>
    public static IReadOnlyList<string>? FindPathToCode(string codeSystemJson, string targetCode)
    {
        using var doc = JsonDocument.Parse(codeSystemJson);
        if (!doc.RootElement.TryGetProperty("concept", out var rootConcepts) || rootConcepts.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var c in rootConcepts.EnumerateArray())
        {
            var path = new List<string>();
            if (WalkFind(c, targetCode, path))
                return path;
        }

        return null;
    }

    private static bool WalkFind(JsonElement node, string targetCode, List<string> path)
    {
        var code = node.TryGetProperty("code", out var ce) ? ce.GetString() : null;
        if (code is null) return false;
        path.Add(code);
        if (code == targetCode)
            return true;

        if (node.TryGetProperty("concept", out var children) && children.ValueKind == JsonValueKind.Array)
        {
            foreach (var ch in children.EnumerateArray())
            {
                if (WalkFind(ch, targetCode, path))
                    return true;
            }
        }

        path.RemoveAt(path.Count - 1);
        return false;
    }

    /// <summary>回傳 FHIR CodeSystem $subsumes 之 outcome 字串：equivalent、subsumes、subsumed-by、not-subsumed、not-applicable。</summary>
    public static string ComputeSubsumes(string codeSystemJson, string codeA, string codeB)
    {
        var pathA = FindPathToCode(codeSystemJson, codeA);
        var pathB = FindPathToCode(codeSystemJson, codeB);
        if (pathA is null || pathB is null)
            return "not-applicable";

        if (codeA == codeB)
            return "equivalent";

        var iaOnB = IndexOf(pathB, codeA);
        var ibOnB = IndexOf(pathB, codeB);
        if (iaOnB >= 0 && ibOnB >= 0 && iaOnB < ibOnB)
            return "subsumes";

        var ibOnA = IndexOf(pathA, codeB);
        var iaOnA = IndexOf(pathA, codeA);
        if (ibOnA >= 0 && iaOnA >= 0 && ibOnA < iaOnA)
            return "subsumed-by";

        return "not-subsumed";
    }

    private static int IndexOf(IReadOnlyList<string> path, string code)
    {
        for (var i = 0; i < path.Count; i++)
        {
            if (path[i] == code)
                return i;
        }

        return -1;
    }
}
