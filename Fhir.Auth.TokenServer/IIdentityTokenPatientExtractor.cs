namespace Fhir.Auth.TokenServer;

/// <summary>自 id_token JWT 取出 patient context（不解耦 JWKS 時可用 payload 探測；正式環境請換加密驗證器）。</summary>
public interface IIdentityTokenPatientExtractor
{
    /// <summary>若 JWT payload 含 FHIR patient 參照或字串 id，回傳簡化後之 patient id／URL。</summary>
    string? TryExtractPatient(string? idTokenJwt);
}
