using Fhir.TypeFramework.Tests.Application.Contracts;
using Reqnroll;

namespace Fhir.TypeFramework.Tests.StepDefinitions;

[Binding]
public sealed class SerializationSteps
{
    private readonly ISerializationBehaviorSuite _suite;

    public SerializationSteps(ISerializationBehaviorSuite suite) => _suite = suite;

    [When(@"I verify primitive JSON serialization")]
    public void WhenIVerifyPrimitiveJsonSerialization() => _suite.VerifyPrimitiveSerialization();

    [When(@"I verify JSON serialization using the Base overload")]
    public void WhenIVerifyBaseOverload() => _suite.VerifySerializeBaseOverload();

    [When(@"I verify serializer options and round-trip deserialization")]
    public void WhenIVerifyRoundTrip() => _suite.VerifyOptionsAndRoundTrip();

    [When(@"I verify Extension JSON contains url")]
    public void WhenIVerifyExtensionUrl() => _suite.VerifyExtensionSerializesWithUrlKey();
}
