using System.Text;
using FhirResourceCreator.Models;

namespace FhirResourceCreator.StructureDefinition;

/// <summary>
/// Converts StructureDefinition.snapshot.element rows into <see cref="ElementRecord"/> rows compatible with the generator (same shape as legacy Excel).
/// </summary>
public static class SnapshotToElementRecords
{
    public static IReadOnlyList<ElementRecord> Convert(StructureDefinitionDocument doc, string resourceTypeName)
    {
        var rows = doc.Snapshot?.Element;
        if (rows == null || rows.Count == 0)
            return Array.Empty<ElementRecord>();

        var deduped = new Dictionary<string, ElementDefinitionModel>(StringComparer.Ordinal);
        foreach (var el in rows)
        {
            if (string.IsNullOrEmpty(el.Path))
                continue;

            if (!deduped.TryGetValue(el.Path, out var existing))
            {
                deduped[el.Path] = el;
                continue;
            }

            deduped[el.Path] = PreferSnapshotElement(existing, el);
        }

        var list = new List<ElementRecord>();
        foreach (var el in deduped.Values)
        {
            var typeString = BuildTypeString(el);
            if (string.IsNullOrEmpty(typeString))
                continue;

            var max = string.IsNullOrEmpty(el.Max) ? "1" : el.Max!;
            list.Add(new ElementRecord(el.Path!, el.Min.ToString(), max, typeString));
        }

        return list;
    }

    static string BuildTypeString(ElementDefinitionModel el)
    {
        // Expanded snapshots keep children under the logical path; treat as backbone so POCOs deserialize nested JSON.
        if (!string.IsNullOrEmpty(el.ContentReference))
            return "BackboneElement";

        var types = el.Type;
        if (types == null || types.Count == 0)
            return string.Empty;

        if (types.Count == 1)
            return SingleType(types[0]);

        var sb = new StringBuilder();
        for (var i = 0; i < types.Count; i++)
        {
            if (i > 0) sb.Append('|');
            sb.Append(SingleType(types[i]));
        }
        return sb.ToString();
    }

    static string SingleType(ElementTypeRef t)
    {
        var code = t.Code ?? "";
        if (code == "Reference")
        {
            var targets = t.TargetProfile?
                .Select(ProfileTail)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList();
            if (targets is { Count: > 0 })
                return $"Reference({string.Join("|", targets)})";
        }

        return code;
    }

    /// <summary>Official snapshots may list the same path twice; merge so generators emit one property.</summary>
    static ElementDefinitionModel PreferSnapshotElement(ElementDefinitionModel a, ElementDefinitionModel b)
    {
        static bool IsStar(string? max) => string.Equals(max, "*", StringComparison.Ordinal);

        if (IsStar(a.Max) && !IsStar(b.Max))
            return a;
        if (IsStar(b.Max) && !IsStar(a.Max))
            return b;

        var ac = a.Type?.Count ?? 0;
        var bc = b.Type?.Count ?? 0;
        if (bc > ac)
            return b;
        return a;
    }

    static string ProfileTail(string uri)
    {
        var i = uri.LastIndexOf('/');
        return i >= 0 && i < uri.Length - 1 ? uri[(i + 1)..] : uri;
    }
}
