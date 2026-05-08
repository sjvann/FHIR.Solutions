using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Fhir.Auth.TokenServer;

/// <summary>OAuth 2.0 JWT Bearer Client Authentication（<c>client_assertion</c>）。</summary>
public static class JwtClientAssertionBuilder
{
    /// <summary>建立簽章後的 client assertion JWT（<c>RS256</c> 或 <c>ES256</c>）。</summary>
    public static string CreateAssertionJwt(string clientId, string tokenEndpointUrl, string privateKeyPem, TimeSpan lifetime)
    {
        var signing = CreateSigningCredentials(privateKeyPem);
        var now = DateTime.UtcNow;
        var handler = new JwtSecurityTokenHandler();

        var token = new JwtSecurityToken(
            issuer: clientId,
            audience: tokenEndpointUrl,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, clientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            },
            notBefore: now,
            expires: now.Add(lifetime),
            signingCredentials: signing);

        return handler.WriteToken(token);
    }

    private static SigningCredentials CreateSigningCredentials(string privateKeyPem)
    {
        try
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);
            return new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        }
        catch
        {
            try
            {
                var ec = ECDsa.Create();
                ec.ImportFromPem(privateKeyPem);
                return new SigningCredentials(new ECDsaSecurityKey(ec), SecurityAlgorithms.EcdsaSha256);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Could not load PEM as RSA or EC private key.", ex);
            }
        }
    }
}
