namespace Fhir.Auth.TokenServer;

/// <summary>預設不啟動背景刷新；請在主程式或獨立 Token Server 替換實作。</summary>
public sealed class NoOpTokenBackgroundRefresher : ITokenBackgroundRefresher
{
    public void Start(ITokenServer tokenServer, CancellationToken hostCancellationToken)
    {
        _ = tokenServer;
        _ = hostCancellationToken;
    }

    public void Stop()
    {
    }
}
