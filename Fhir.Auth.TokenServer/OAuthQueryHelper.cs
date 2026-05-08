using System.Text;

namespace Fhir.Auth.TokenServer;

internal static class OAuthQueryHelper
{
    public static string AppendQuery(string url, IReadOnlyDictionary<string, string> parameters)
    {
        if (parameters.Count == 0)
            return url;

        var sb = new StringBuilder();
        var first = !url.Contains('?', StringComparison.Ordinal);
        foreach (var kv in parameters)
        {
            sb.Append(first ? '?' : '&');
            first = false;
            sb.Append(Uri.EscapeDataString(kv.Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(kv.Value));
        }

        return url + sb;
    }

    public static Dictionary<string, string> ParseQuery(string? query)
    {
        var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(query))
            return d;

        var q = query.StartsWith('?') ? query[1..] : query;
        foreach (var segment in q.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var eq = segment.IndexOf('=');
            if (eq <= 0)
                continue;

            var key = Uri.UnescapeDataString(segment[..eq]);
            var val = eq < segment.Length - 1
                ? Uri.UnescapeDataString(segment[(eq + 1)..])
                : "";

            d[key] = val;
        }

        return d;
    }
}
