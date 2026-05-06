namespace Fhir.TypeFramework.Tests.Application.Contracts;

/// <summary>
/// JSON 序列化／反序列化行為（對應 <see cref="Fhir.TypeFramework.Serialization.FhirJsonSerializer"/>）。
/// </summary>
public interface ISerializationBehaviorSuite
{
    void VerifyPrimitiveSerialization();

    void VerifySerializeBaseOverload();

    void VerifyOptionsAndRoundTrip();

    void VerifyExtensionSerializesWithUrlKey();
}
