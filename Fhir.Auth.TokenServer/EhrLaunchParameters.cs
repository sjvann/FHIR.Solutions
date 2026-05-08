namespace Fhir.Auth.TokenServer;

/// <summary>EHR 啟動應用程式時傳入之 SMART launch 參數（與桌面 standalone 授權分流）。</summary>
public sealed class EhrLaunchParameters
{
    /// <summary>FHIR 伺服器 base URL（iss）。</summary>
    public required string Iss { get; init; }

    /// <summary>EHR 發給應用程式的一次性 launch token。</summary>
    public required string Launch { get; init; }

    public string? Scopes { get; init; }

    public string? ClientId { get; init; }
}
