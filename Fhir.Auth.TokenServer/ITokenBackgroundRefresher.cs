namespace Fhir.Auth.TokenServer;

/// <summary>背景依 refresh_token 更新 access_token（排程／長連線）；由 Token Server 主機託管，非 Query Builder。</summary>
public interface ITokenBackgroundRefresher
{
    /// <summary>登入成功後可呼叫；預設 <see cref="NoOpTokenBackgroundRefresher"/> 不做事。</summary>
    void Start(ITokenServer tokenServer, CancellationToken hostCancellationToken);

    void Stop();
}
