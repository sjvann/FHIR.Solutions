using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Serialization;

namespace Fhir.Resource.Tests.Common.Serialization;

/// <summary>FHIR JSON via <see cref="FhirJsonSerializer"/> (System.Text.Json).</summary>
public sealed class FhirJsonWireCodec<T> : IResourceWireCodec<T> where T : DomainResource
{
    public string WireFormat => "json";

    public bool IsSupported => true;

    public T? Parse(string payload) => FhirJsonSerializer.Deserialize<T>(payload);

    public string Write(T value) => FhirJsonSerializer.Serialize(value);
}
