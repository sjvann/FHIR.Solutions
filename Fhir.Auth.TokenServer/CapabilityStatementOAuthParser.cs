using System.Text.Json;

namespace Fhir.Auth.TokenServer;

/// <summary>從 <c>/metadata</c> CapabilityStatement JSON 擷取 SMART OAuth URIs（後援於 well-known）。</summary>
public static class CapabilityStatementOAuthParser
{
    /// <summary>常見 extension URL（含 SMART registry；部分伺服器使用 HL7 正式 canonical）。</summary>
    private static readonly string[] OauthUrisMarkers =
    [
        "StructureDefinition/oauth-uris",
        "oauth-uris",
    ];

    /// <summary>若 JSON 含 oauth-uris extension，回傳 authorize／token（可能其一為 null）。</summary>
    public static (string? Authorize, string? Token) TryExtractEndpoints(string capabilityStatementJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(capabilityStatementJson);
            string? authorize = null;
            string? token = null;
            Walk(doc.RootElement, ref authorize, ref token);
            return (authorize, token);
        }
        catch
        {
            return (null, null);
        }
    }

    private static void Walk(JsonElement el, ref string? authorize, ref string? token)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Object:
                if (TryParseOAuthUrisExtension(el, ref authorize, ref token))
                    return;

                foreach (var p in el.EnumerateObject())
                    Walk(p.Value, ref authorize, ref token);
                break;

            case JsonValueKind.Array:
                foreach (var item in el.EnumerateArray())
                    Walk(item, ref authorize, ref token);
                break;
        }
    }

    private static bool TryParseOAuthUrisExtension(JsonElement obj, ref string? authorize, ref string? token)
    {
        if (!obj.TryGetProperty("url", out var urlProp) || urlProp.ValueKind != JsonValueKind.String)
            return false;

        var url = urlProp.GetString();
        if (url == null || !OauthUrisMarkers.Any(m => url.Contains(m, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (!obj.TryGetProperty("extension", out var inner) || inner.ValueKind != JsonValueKind.Array)
            return false;

        foreach (var ext in inner.EnumerateArray())
        {
            if (ext.ValueKind != JsonValueKind.Object)
                continue;

            if (!ext.TryGetProperty("url", out var uEl) || uEl.ValueKind != JsonValueKind.String)
                continue;

            var u = uEl.GetString();
            if (u == null)
                continue;

            if (u.Equals("authorize", StringComparison.OrdinalIgnoreCase) &&
                ext.TryGetProperty("valueUri", out var vAuth) && vAuth.ValueKind == JsonValueKind.String)
                authorize = vAuth.GetString();

            if (u.Equals("token", StringComparison.OrdinalIgnoreCase) &&
                ext.TryGetProperty("valueUri", out var vTok) && vTok.ValueKind == JsonValueKind.String)
                token = vTok.GetString();
        }

        return authorize != null || token != null;
    }
}
