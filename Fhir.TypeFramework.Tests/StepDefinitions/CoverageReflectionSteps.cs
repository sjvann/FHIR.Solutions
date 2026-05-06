using Fhir.TypeFramework.Tests.Application.Contracts;
using Reqnroll;

namespace Fhir.TypeFramework.Tests.StepDefinitions;

[Binding]
public sealed class CoverageReflectionSteps
{
    private readonly ICoverageReflectionBehaviorSuite _suite;

    public CoverageReflectionSteps(ICoverageReflectionBehaviorSuite suite) => _suite = suite;

    [When(@"I touch all data type properties for coverage")]
    public void WhenTouchDataTypes() => _suite.TouchAllDataTypesProperties();

    [When(@"I exercise all concrete base derived types")]
    public void WhenExerciseBaseTypes() => _suite.ExerciseAllConcreteBaseDerivedTypes();

    [When(@"I exercise primitive type JSON helpers")]
    public void WhenExerciseJsonHelpers() => _suite.ExercisePrimitiveTypeJsonHelpers();

    [When(@"I exercise performance and serialization utilities")]
    public void WhenExercisePerfSerialization() => _suite.ExercisePerformanceAndSerializationUtilities();

    [When(@"I invoke most public methods via reflection")]
    public void WhenInvokePublicMethods() => _suite.ReflectionInvokeMostPublicMethods();
}
