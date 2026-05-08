namespace Fhir.Auth.TokenServer;

/// <summary>預設占位：請改為註冊自訂 <see cref="IEhrLaunchCoordinator"/>（連線您的主機／流程）。</summary>
public sealed class UnsupportedEhrLaunchCoordinator : IEhrLaunchCoordinator
{
    public Task<SmartTokenResult> ExecuteLaunchFlowAsync(EhrLaunchParameters parameters,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        return Task.FromResult(SmartTokenResult.Failed("ehr_launch_not_implemented",
            "請使用獨立 Token Server 註冊 IEhrLaunchCoordinator，或改用 standalone PKCE。"));
    }
}
