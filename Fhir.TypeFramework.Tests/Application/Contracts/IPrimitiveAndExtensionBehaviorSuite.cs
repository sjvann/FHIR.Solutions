namespace Fhir.TypeFramework.Tests.Application.Contracts;

/// <summary>
/// 基本型別、Extension 與擴充方法相關行為。
/// </summary>
public interface IPrimitiveAndExtensionBehaviorSuite
{
    void VerifyIntegerValidateSkipsWhenStringValueNull();

    void VerifyExtensionValidationErrorsForInvalidId();

    void VerifyExtensionRequiresUrl();

    void VerifyPrimitiveIValueSemantics();

    void VerifyExtensionCrudOnElement();

    void VerifyValidateAndThrowForInvalidId();
}
