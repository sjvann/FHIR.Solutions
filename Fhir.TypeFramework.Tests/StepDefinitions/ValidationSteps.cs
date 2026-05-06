using Fhir.TypeFramework.Tests.Application.Contracts;
using Reqnroll;

namespace Fhir.TypeFramework.Tests.StepDefinitions;

[Binding]
public sealed class ValidationSteps
{
    private readonly IValidationBehaviorSuite _suite;

    public ValidationSteps(IValidationBehaviorSuite suite) => _suite = suite;

    [When(@"I verify FHIR id validation rules")]
    public void WhenIVerifyFhirIdValidationRules() => _suite.VerifyFhirIdRules();

    [When(@"I verify basic validation helpers")]
    public void WhenIVerifyBasicValidationHelpers() => _suite.VerifyBasicValidationHelpers();

    [When(@"I verify FHIR URI and FHIR code validation branches")]
    public void WhenIVerifyFhirUriAndCode() => _suite.VerifyFhirUriAndCodeBranches();
}
