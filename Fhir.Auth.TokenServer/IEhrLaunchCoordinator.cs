namespace Fhir.Auth.TokenServer;

/// <summary>EHR Launch（<c>launch</c>／<c>iss</c>）換發 token；由 Token Server 主機實作，Query Builder 僅可選注入呼叫。</summary>
public interface IEhrLaunchCoordinator
{
    Task<SmartTokenResult> ExecuteLaunchFlowAsync(EhrLaunchParameters parameters,
        CancellationToken cancellationToken = default);
}
