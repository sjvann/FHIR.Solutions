using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Serialization;
using Fhir.Resource.Tests.Common.Serialization;

namespace Fhir.Resource.Tests.Common;

/// <summary>
/// Convenience helpers for JSON round-trip at the I/O boundary (same POCO graph as application code).
/// Prefer <see cref="Serialization.FhirJsonWireCodec{T}"/> when injecting codecs into tests.
/// </summary>
public static class FhirJsonRoundTrip
{
    public static string Serialize<T>(T value) where T : Base => FhirJsonSerializer.Serialize(value);

    public static T? Deserialize<T>(string json) where T : Base => FhirJsonSerializer.Deserialize<T>(json);

    public static T RoundTrip<T>(T value) where T : Base
    {
        var json = Serialize(value);
        return Deserialize<T>(json)!;
    }

    /// <summary>Serialize then deserialize; useful for asserting mutable POCOs survive a boundary trip.</summary>
    public static T RoundTripDomain<T>(T value, FhirJsonWireCodec<T>? codec = null)
        where T : DomainResource
    {
        codec ??= new FhirJsonWireCodec<T>();
        return codec.Parse(codec.Write(value))!;
    }
}
