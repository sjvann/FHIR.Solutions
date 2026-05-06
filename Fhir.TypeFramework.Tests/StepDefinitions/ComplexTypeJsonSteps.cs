using Fhir.TypeFramework.Tests.Application.Contracts;
using Reqnroll;

namespace Fhir.TypeFramework.Tests.StepDefinitions;

[Binding]
public sealed class ComplexTypeJsonSteps
{
    private readonly IComplexTypeJsonBehaviorSuite _suite;

    public ComplexTypeJsonSteps(IComplexTypeJsonBehaviorSuite suite) => _suite = suite;

    [When(@"I verify all complex types deserialize from FHIR JSON")]
    public void WhenDeserializeAll() => _suite.VerifyAllComplexTypesDeserializeFromFhirJson();

    [When(@"I verify all complex types serialize to FHIR-shaped JSON")]
    public void WhenSerializeAll() => _suite.VerifyAllComplexTypesSerializeToFhirShapedJson();

    [When(@"I verify all complex types JSON round-trip")]
    public void WhenRoundTripAll() => _suite.VerifyAllComplexTypesRoundTrip();

    [When(@"I verify primitive JSON converter legacy object form")]
    public void WhenLegacy() => _suite.VerifyPrimitiveJsonConverterLegacyObjectForm();

    [When(@"I verify primitive JSON converter boolean and number tokens")]
    public void WhenBoolNumber() => _suite.VerifyPrimitiveJsonConverterReadsBooleanAndNumber();

    [When(@"I verify primitive JSON converter null long and scientific")]
    public void WhenNullLongSci() => _suite.VerifyPrimitiveJsonConverterReadsNullLongAndScientific();

    [When(@"I verify primitive JSON converter numeric shapes")]
    public void WhenNumericShapes() => _suite.VerifyPrimitiveJsonConverterNumericShapes();
}
