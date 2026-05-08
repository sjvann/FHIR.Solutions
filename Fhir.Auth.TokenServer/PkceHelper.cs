using System.Security.Cryptography;
using System.Text;

namespace Fhir.Auth.TokenServer;

/// <summary>RFC 7636 PKCE：<c>code_verifier</c>／<c>S256</c> challenge。</summary>
public static class PkceHelper
{
    public static string CreateCodeVerifier()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Base64UrlEncode(bytes);
    }

    public static string CreateChallengeS256(string codeVerifier)
    {
        var ascii = Encoding.ASCII.GetBytes(codeVerifier);
        var hash = SHA256.HashData(ascii);
        return Base64UrlEncode(hash);
    }

    public static string CreateState()
    {
        var bytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToHexString(bytes);
    }

    private static string Base64UrlEncode(byte[] data) =>
        Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
