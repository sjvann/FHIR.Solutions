using Fhir.TypeFramework.Tests.Application.Contracts;
using Reqnroll;

namespace Fhir.TypeFramework.Tests.StepDefinitions;

[Binding]
public sealed class PerformanceSteps
{
    private readonly IPerformanceBehaviorSuite _suite;

    public PerformanceSteps(IPerformanceBehaviorSuite suite) => _suite = suite;

    [When(@"I verify deep copy optimizer and monitoring")]
    public void WhenDeepCopy() => _suite.VerifyDeepCopyOptimizerAndMonitoring();

    [When(@"I verify validation optimizer batch APIs")]
    public void WhenBatch() => _suite.VerifyValidationOptimizerBatchApis();

    [When(@"I verify type framework cache get-or-add and clear")]
    public void WhenCache() => _suite.VerifyTypeFrameworkCacheGetOrAddAndClear();
}
