using System.Text.Json;

namespace Fhir.QueryBuilder.Services;

/// <summary>自 FHIR Bundle／SearchParameter JSON 解析 composite 之 component.definition canonical。</summary>
public static class SearchParameterCompositeParser
{
    /// <summary>
    /// 自 Search Bundle JSON 取出第一個含 <c>component</c>（至少兩筆）之 SearchParameter 的 definition URLs。
    /// </summary>
    public static IReadOnlyList<string>? TryGetComponentDefinitionCanonicals(string bundleJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(bundleJson);
            var root = doc.RootElement;
            if (!string.Equals(root.GetProperty("resourceType").GetString(), "Bundle", StringComparison.Ordinal))
                return null;

            if (!root.TryGetProperty("entry", out var entries))
                return null;

            foreach (var entry in entries.EnumerateArray())
            {
                if (!entry.TryGetProperty("resource", out var res))
                    continue;
                if (!string.Equals(res.GetProperty("resourceType").GetString(), "SearchParameter", StringComparison.Ordinal))
                    continue;
                if (!res.TryGetProperty("component", out var compArr))
                    continue;

                var list = new List<string>();
                foreach (var c in compArr.EnumerateArray())
                {
                    if (!c.TryGetProperty("definition", out var def))
                        continue;
                    var s = GetCanonicalString(def);
                    if (!string.IsNullOrEmpty(s))
                        list.Add(s);
                }

                if (list.Count >= 2)
                    return list;
            }
        }
        catch (JsonException)
        {
        }

        return null;
    }

    /// <summary>自 SearchParameter JSON（單一資源）讀取 <c>type</c>。</summary>
    public static string? TryGetSearchParameterType(string searchParameterJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(searchParameterJson);
            var root = doc.RootElement;
            if (!string.Equals(root.GetProperty("resourceType").GetString(), "SearchParameter", StringComparison.Ordinal))
                return null;
            if (!root.TryGetProperty("type", out var t))
                return null;
            return t.ValueKind == JsonValueKind.String ? t.GetString() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>自 Bundle JSON 取第一筆 SearchParameter 之 type。</summary>
    public static string? TryGetFirstSearchParameterTypeFromBundle(string bundleJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(bundleJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("entry", out var entries))
                return null;

            foreach (var entry in entries.EnumerateArray())
            {
                if (!entry.TryGetProperty("resource", out var res))
                    continue;
                if (!string.Equals(res.GetProperty("resourceType").GetString(), "SearchParameter", StringComparison.Ordinal))
                    continue;
                if (!res.TryGetProperty("type", out var t))
                    continue;
                return t.ValueKind == JsonValueKind.String ? t.GetString() : null;
            }
        }
        catch (JsonException)
        {
        }

        return null;
    }

    private static string? GetCanonicalString(JsonElement def)
    {
        return def.ValueKind switch
        {
            JsonValueKind.String => def.GetString(),
            JsonValueKind.Object when def.TryGetProperty("value", out var v) && v.ValueKind == JsonValueKind.String =>
                v.GetString(),
            _ => null
        };
    }
}
