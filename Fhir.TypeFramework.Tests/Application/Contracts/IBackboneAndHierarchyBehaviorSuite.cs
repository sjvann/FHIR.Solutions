namespace Fhir.TypeFramework.Tests.Application.Contracts;

/// <summary>
/// Backbone／Resource／ComplexType 等階層與基底類別行為。
/// </summary>
public interface IBackboneAndHierarchyBehaviorSuite
{
    void VerifyBackboneElementAndBackboneTypeModifierExtensions();

    void VerifyBackboneElementIsExactlyAndValidate();

    void VerifyBaseExplicitITypeFrameworkMethods();

    void VerifyComplexTypeBaseDeepCopyList();

    void VerifyBooleanPrimitiveParsingAndHelpers();

    void VerifyElementPrimitiveNumericDateTimeBaseClasses();

    void VerifyStringPrimitiveAndDataTypeBranches();

    void VerifyResourceIsExactlyAndValidateBranches();

    void VerifyCoverBaseComplexTypeResourceAndMessages();

    void VerifyCoverElementBackboneTypeResourceValidateBranches();
}
