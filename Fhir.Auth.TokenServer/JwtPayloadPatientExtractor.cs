using System.Text;
using System.Text.Json;

namespace Fhir.Auth.TokenServer;

/// <summary>僅 Base64 解碼 JWT payload（不驗簽）；正式環境請改為 JWKS 驗證後再解。</summary>
public sealed class JwtPayloadPatientExtractor : IIdentityTokenPatientExtractor
{
    public string? TryExtractPatient(string? idTokenJwt)
    {
        if (string.IsNullOrWhiteSpace(idTokenJwt))
            return null;

        var parts = idTokenJwt.Split('.');
        if (parts.Length < 2)
            return null;

        try
        {
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;

            if (root.TryGetProperty("patient", out var p))
            {
                if (p.ValueKind == JsonValueKind.String)
                    return p.GetString();

                if (p.ValueKind == JsonValueKind.Object &&
                    p.TryGetProperty("reference", out var r) &&
                    r.ValueKind == JsonValueKind.String)
                    return r.GetString();
            }

            if (root.TryGetProperty("fhirUser", out var fu) && fu.ValueKind == JsonValueKind.String)
                return fu.GetString();

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static byte[] Base64UrlDecode(string segment)
    {
        var s = segment.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2:
                s += "==";
                break;
            case 3:
                s += "=";
                break;
        }

        return Convert.FromBase64String(s);
    }
}
