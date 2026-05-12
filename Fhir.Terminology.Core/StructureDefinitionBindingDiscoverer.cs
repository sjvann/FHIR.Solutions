using System.Text.Json;

namespace Fhir.Terminology.Core;

/// <summary>
/// 自 <see cref="StructureDefinition"/> JSON（通常含 <c>snapshot</c> 或 <c>differential</c>）擷取元素綁定（terminology binding）。
/// </summary>
public static class StructureDefinitionBindingDiscoverer
{
    /// <summary>取 canonical URL（<c>StructureDefinition.url</c>），供與 <c>ValueSet/$validate-code?profile=…</c> 對應。</summary>
    public static string? TryGetCanonicalUrl(string rawJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            return TryGetCanonicalUrl(doc.RootElement);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>列出所有綁定（含主 <c>valueSet</c> 與 <c>additional</c>），供 UI 追溯。</summary>
    public static IReadOnlyList<StructureDefinitionDiscoveredBinding> ListAllBindings(string rawJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            return Collect(doc.RootElement, includeAdditional: true);
        }
        catch (JsonException)
        {
            return [];
        }
    }

    /// <summary>
    /// 每個元素 path 只取<strong>主</strong> <c>binding.valueSet</c>（不含 <c>additional</c>），供寫入驗證用 Binding 登錄；
    /// 與驗證端點 <c>ValueSet/$validate-code?profile&amp;path</c> 使用之 Binding 登錄一致（每 path 僅建議一筆主 ValueSet）。
    /// </summary>
    public static IReadOnlyList<StructureDefinitionPrimaryBinding> ListPrimaryBindingsForRegistry(string rawJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var all = Collect(doc.RootElement, includeAdditional: false);
            return all
                .GroupBy(x => x.ElementPath, StringComparer.Ordinal)
                .Select(g => g.First())
                .Select(x => new StructureDefinitionPrimaryBinding(x.ElementPath, x.Strength, x.ValueSetCanonical))
                .ToList();
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static string? TryGetCanonicalUrl(JsonElement root)
    {
        if (!root.TryGetProperty("url", out var urlEl))
            return null;
        return ReadCanonicalString(urlEl);
    }

    private static List<StructureDefinitionDiscoveredBinding> Collect(JsonElement root, bool includeAdditional)
    {
        var list = new List<StructureDefinitionDiscoveredBinding>();
        var elements = PickElementArray(root);
        if (elements is null)
            return list;

        foreach (var el in elements.Value.EnumerateArray())
        {
            if (!el.TryGetProperty("path", out var pathProp) || pathProp.ValueKind != JsonValueKind.String)
                continue;
            var path = pathProp.GetString();
            if (string.IsNullOrEmpty(path))
                continue;
            if (!el.TryGetProperty("binding", out var binding))
                continue;

            var strength = ReadString(binding, "strength") ?? "";

            foreach (var vs in EnumerateBindingValueSetCanonicals(binding))
                list.Add(new StructureDefinitionDiscoveredBinding(path, strength, vs, StructureDefinitionBindingSlot.Primary));

            if (includeAdditional && binding.TryGetProperty("additional", out var additional)
                                   && additional.ValueKind == JsonValueKind.Array)
            {
                foreach (var add in additional.EnumerateArray())
                {
                    if (!add.TryGetProperty("valueSet", out var avs))
                        continue;
                    foreach (var vs in EnumerateCanonicalStrings(avs))
                    {
                        if (!string.IsNullOrEmpty(vs))
                            list.Add(new StructureDefinitionDiscoveredBinding(path, strength, vs, StructureDefinitionBindingSlot.Additional));
                    }
                }
            }
        }

        return list;
    }

    /// <summary>優先使用 <c>snapshot.element</c>（與執行期語意一致）；若無則 <c>differential.element</c>。</summary>
    private static JsonElement? PickElementArray(JsonElement root)
    {
        static JsonElement? Elements(JsonElement section)
        {
            if (!section.TryGetProperty("element", out var arr) || arr.ValueKind != JsonValueKind.Array || arr.GetArrayLength() == 0)
                return null;
            return arr;
        }

        if (root.TryGetProperty("snapshot", out var snap))
        {
            var e = Elements(snap);
            if (e.HasValue)
                return e;
        }

        if (root.TryGetProperty("differential", out var diff))
        {
            var e = Elements(diff);
            if (e.HasValue)
                return e;
        }

        return null;
    }

    /// <summary>FHIR canonical 可能含 <c>|version</c>；查本地 ValueSet 時可先截斷。</summary>
    public static string StripPipeVersionSuffix(string canonical)
    {
        var i = canonical.IndexOf('|', StringComparison.Ordinal);
        return i < 0 ? canonical : canonical[..i];
    }

    /// <summary>主 binding 的 valueSet（不含 additional）。</summary>
    private static IEnumerable<string> EnumerateBindingValueSetCanonicals(JsonElement binding)
    {
        if (!binding.TryGetProperty("valueSet", out var vsEl))
            yield break;

        foreach (var s in EnumerateCanonicalStrings(vsEl))
        {
            if (!string.IsNullOrEmpty(s))
                yield return s;
        }
    }

    private static IEnumerable<string> EnumerateCanonicalStrings(JsonElement vsEl)
    {
        switch (vsEl.ValueKind)
        {
            case JsonValueKind.String:
                var s = vsEl.GetString();
                if (!string.IsNullOrEmpty(s))
                    yield return s.Trim();
                yield break;
            case JsonValueKind.Array:
                foreach (var item in vsEl.EnumerateArray())
                {
                    foreach (var inner in EnumerateCanonicalStrings(item))
                        yield return inner;
                }

                yield break;
            case JsonValueKind.Object:
                // 少數序列化可能包一層 {"value": "http://..."}
                if (vsEl.TryGetProperty("value", out var val))
                {
                    foreach (var inner in EnumerateCanonicalStrings(val))
                        yield return inner;
                }

                yield break;
            default:
                yield break;
        }
    }

    private static string? ReadString(JsonElement el, string name) =>
        el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;

    private static string? ReadCanonicalString(JsonElement urlEl) =>
        urlEl.ValueKind == JsonValueKind.String ? urlEl.GetString()?.Trim() : null;
}

/// <summary>UI 用的單筆綁定（可含 additional）。</summary>
public sealed record StructureDefinitionDiscoveredBinding(
    string ElementPath,
    string Strength,
    string ValueSetCanonical,
    StructureDefinitionBindingSlot Slot);

public enum StructureDefinitionBindingSlot
{
    Primary,
    Additional,
}

/// <summary>寫入 Binding 登錄之一筆（每 path 僅主 valueSet）。</summary>
public sealed record StructureDefinitionPrimaryBinding(string ElementPath, string Strength, string ValueSetCanonical);
