namespace Fhir.TypeFramework.Tests.Application.Contracts;

/// <summary>
/// 效能與批次驗證、快取相關行為。
/// </summary>
public interface IPerformanceBehaviorSuite
{
    void VerifyDeepCopyOptimizerAndMonitoring();

    void VerifyValidationOptimizerBatchApis();

    void VerifyTypeFrameworkCacheGetOrAddAndClear();
}
