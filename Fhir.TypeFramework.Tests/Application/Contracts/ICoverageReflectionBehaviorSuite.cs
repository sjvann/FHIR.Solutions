namespace Fhir.TypeFramework.Tests.Application.Contracts;

/// <summary>
/// 以反射／大量探測方式補齊覆蓋率的行為（與業務規格較弱相關，集中管理避免污染步驟定義）。
/// </summary>
public interface ICoverageReflectionBehaviorSuite
{
    void TouchAllDataTypesProperties();

    void ExerciseAllConcreteBaseDerivedTypes();

    void ExercisePrimitiveTypeJsonHelpers();

    void ExercisePerformanceAndSerializationUtilities();

    void ReflectionInvokeMostPublicMethods();
}
