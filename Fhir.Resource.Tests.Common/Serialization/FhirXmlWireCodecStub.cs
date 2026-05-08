using Fhir.TypeFramework.Bases;

namespace Fhir.Resource.Tests.Common.Serialization;

/// <summary>
/// Placeholder until FHIR XML read/write exists in Fhir.TypeFramework. Do not use for production interchange.
/// </summary>
public sealed class FhirXmlWireCodecStub<T> : IResourceWireCodec<T> where T : DomainResource
{
    public string WireFormat => "xml";

    public bool IsSupported => false;

    public T? Parse(string payload) =>
        throw new NotSupportedException(
            "FHIR XML parsing is not implemented yet. Use FhirJsonWireCodec<T> or parse JSON into the same POCO graph.");

    public string Write(T value) =>
        throw new NotSupportedException(
            "FHIR XML serialization is not implemented yet. Serialize to JSON from the same object graph.");
}
