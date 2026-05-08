using System.Text.Json;

namespace Fhir.QueryBuilder.Metadata;

/// <summary>
/// 自 CapabilityStatement JSON 擷取 <c>rest[].resource[].type</c>（POCO 反序列化不完整時之後援）。
/// </summary>
public static class CapabilityResourceTypeExtractor
{
    public static IReadOnlyList<string>? FromCapabilityJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("rest", out var restEl) || restEl.ValueKind != JsonValueKind.Array)
                return null;

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var rest in restEl.EnumerateArray())
            {
                if (!rest.TryGetProperty("resource", out var resourceArr) ||
                    resourceArr.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var entry in resourceArr.EnumerateArray())
                {
                    if (!entry.TryGetProperty("type", out var typeEl))
                        continue;

                    string? code = null;
                    if (typeEl.ValueKind == JsonValueKind.String)
                        code = typeEl.GetString();
                    else if (typeEl.ValueKind == JsonValueKind.Object &&
                             typeEl.TryGetProperty("code", out var codeEl) &&
                             codeEl.ValueKind == JsonValueKind.String)
                        code = codeEl.GetString();

                    if (!string.IsNullOrEmpty(code))
                        set.Add(code);
                }
            }

            return set.Count > 0 ? set.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
