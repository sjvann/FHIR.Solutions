namespace Fhir.QueryBuilder.Platform;

/// <summary>舊版 JWT 取得流程之占位（無外部 TokenServices 組件時）。</summary>
internal static class LegacyJwtTokenStub
{
    public static string GetTokenJWT(string baseUrl, string clientId, string privateKey)
    {
        _ = baseUrl;
        _ = clientId;
        _ = privateKey;
        MessageBox.Show(
            "此組建未內建自動 JWT 交換；請於伺服器支援時改用手動貼上 Bearer token，或另行整合 OAuth。",
            "Token",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        return string.Empty;
    }
}
