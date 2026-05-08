namespace Fhir.TypeFramework.Serialization;

/// <summary>
/// FHIR JSON is implemented by <see cref="FhirJsonSerializer"/>. FHIR XML read/write is planned as separate codecs at this same I/O boundary,
/// populating the same POCO object graph (no JSON↔XML string conversion per the FHIR specification).
/// </summary>
/// <remarks>
/// Test projects may use <c>Fhir.Resource.Tests.Common.Serialization.FhirXmlWireCodecStub&lt;T&gt;</c> until real XML support lands here.
/// </remarks>
public static class FhirXmlSerializationBoundary
{
}
