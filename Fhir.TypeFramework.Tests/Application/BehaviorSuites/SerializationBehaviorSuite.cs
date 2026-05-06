using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Fhir.TypeFramework.Serialization;
using Fhir.TypeFramework.Tests.Application.Contracts;

namespace Fhir.TypeFramework.Tests.Application.BehaviorSuites;

public sealed class SerializationBehaviorSuite : ISerializationBehaviorSuite
{
    public void VerifyPrimitiveSerialization()
    {
        var s = new FhirString("abc");
        var json = FhirJsonSerializer.Serialize(s);
        Assert.Contains("abc", json);
    }

    public void VerifySerializeBaseOverload()
    {
        Base b = new FhirString("abc");
        var json = FhirJsonSerializer.Serialize(b);
        Assert.Contains("abc", json);
    }

    public void VerifyOptionsAndRoundTrip()
    {
        _ = FhirJsonSerializer.Options;

        var json = FhirJsonSerializer.Serialize(new FhirString("x"));
        var roundtrip = FhirJsonSerializer.Deserialize<FhirString>(json);
        Assert.Equal("x", roundtrip?.StringValue);
    }

    public void VerifyExtensionSerializesWithUrlKey()
    {
        var ext = new Extension { Url = "http://example.com/ext" };
        var json = FhirJsonSerializer.Serialize(ext);
        Assert.Contains("url", json, StringComparison.OrdinalIgnoreCase);
    }
}
