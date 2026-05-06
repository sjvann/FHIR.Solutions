using Fhir.TypeFramework.Tests.Application.Contracts;
using Reqnroll;

namespace Fhir.TypeFramework.Tests.StepDefinitions;

[Binding]
public sealed class PrimitivesAndExtensionsSteps
{
    private readonly IPrimitiveAndExtensionBehaviorSuite _suite;

    public PrimitivesAndExtensionsSteps(IPrimitiveAndExtensionBehaviorSuite suite) => _suite = suite;

    [When(@"I verify integer validation skips when string value is null")]
    public void WhenIntegerSkips() => _suite.VerifyIntegerValidateSkipsWhenStringValueNull();

    [When(@"I verify extension validation errors for an invalid id")]
    public void WhenInvalidIdErrors() => _suite.VerifyExtensionValidationErrorsForInvalidId();

    [When(@"I verify extension requires url")]
    public void WhenExtensionRequiresUrl() => _suite.VerifyExtensionRequiresUrl();

    [When(@"I verify primitive IValue semantics")]
    public void WhenIValueSemantics() => _suite.VerifyPrimitiveIValueSemantics();

    [When(@"I verify extension CRUD on element")]
    public void WhenExtensionCrud() => _suite.VerifyExtensionCrudOnElement();

    [When(@"I verify validate and throw for invalid id")]
    public void WhenValidateAndThrow() => _suite.VerifyValidateAndThrowForInvalidId();
}
