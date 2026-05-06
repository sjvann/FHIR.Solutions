namespace Fhir.TypeFramework.Tests.Application.Contracts;

/// <summary>
/// 驗證規則相關行為（對應 <see cref="Fhir.TypeFramework.Validation.ValidationFramework"/>）。
/// </summary>
public interface IValidationBehaviorSuite
{
    void VerifyFhirIdRules();

    void VerifyBasicValidationHelpers();

    void VerifyFhirUriAndCodeBranches();
}
