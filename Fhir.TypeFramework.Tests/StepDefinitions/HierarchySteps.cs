using Fhir.TypeFramework.Tests.Application.Contracts;
using Reqnroll;

namespace Fhir.TypeFramework.Tests.StepDefinitions;

[Binding]
public sealed class HierarchySteps
{
    private readonly IBackboneAndHierarchyBehaviorSuite _suite;

    public HierarchySteps(IBackboneAndHierarchyBehaviorSuite suite) => _suite = suite;

    [When(@"I verify backbone element and backbone type modifier extensions")]
    public void WhenBackboneModifier() => _suite.VerifyBackboneElementAndBackboneTypeModifierExtensions();

    [When(@"I verify backbone element equality and validation")]
    public void WhenBackboneEquality() => _suite.VerifyBackboneElementIsExactlyAndValidate();

    [When(@"I verify base explicit ITypeFramework methods")]
    public void WhenBaseExplicit() => _suite.VerifyBaseExplicitITypeFrameworkMethods();

    [When(@"I verify complex type base deep copy list")]
    public void WhenDeepCopyList() => _suite.VerifyComplexTypeBaseDeepCopyList();

    [When(@"I verify boolean primitive parsing and helpers")]
    public void WhenBooleanHelpers() => _suite.VerifyBooleanPrimitiveParsingAndHelpers();

    [When(@"I verify element primitive numeric and datetime base classes")]
    public void WhenElementPrimitives() => _suite.VerifyElementPrimitiveNumericDateTimeBaseClasses();

    [When(@"I verify string primitive and data type branches")]
    public void WhenStringDataType() => _suite.VerifyStringPrimitiveAndDataTypeBranches();

    [When(@"I verify resource equality and validation branches")]
    public void WhenResourceBranches() => _suite.VerifyResourceIsExactlyAndValidateBranches();

    [When(@"I verify cover base complex type resource and messages")]
    public void WhenCoverBase() => _suite.VerifyCoverBaseComplexTypeResourceAndMessages();

    [When(@"I verify cover element backbone type resource validation branches")]
    public void WhenCoverElement() => _suite.VerifyCoverElementBackboneTypeResourceValidateBranches();
}
