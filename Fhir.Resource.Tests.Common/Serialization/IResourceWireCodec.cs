using Fhir.TypeFramework.Bases;

namespace Fhir.Resource.Tests.Common.Serialization;

/// <summary>
/// I/O boundary codec for a FHIR wire format (JSON now; XML later). Same POCO <typeparamref name="T"/> for every format — no string-level format conversion.
/// </summary>
public interface IResourceWireCodec<T> where T : DomainResource
{
    /// <summary>Discriminator, e.g. <c>json</c> or <c>xml</c>.</summary>
    string WireFormat { get; }

    bool IsSupported { get; }

    T? Parse(string payload);

    string Write(T value);
}
