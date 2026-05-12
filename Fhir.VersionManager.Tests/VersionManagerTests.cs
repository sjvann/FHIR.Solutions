using FluentAssertions;
using Fhir.VersionManager.Capability;

namespace Fhir.VersionManager.Tests;

public class FhirVersionParserTests
{
    [Theory]
    [InlineData("4.0.1", FhirVersion.R4)]
    [InlineData("4.3.0", FhirVersion.R4B)]
    [InlineData("4.3.0-snapshot1", FhirVersion.R4B)]
    [InlineData("R4B", FhirVersion.R4B)]
    [InlineData("5.0.0", FhirVersion.R5)]
    [InlineData(null, FhirVersion.Unknown)]
    [InlineData("", FhirVersion.Unknown)]
    public void ParseFromCapabilityString_maps_expected(string? input, FhirVersion expected)
        => FhirVersionParser.ParseFromCapabilityString(input).Should().Be(expected);

    [Theory]
    [InlineData("R4", FhirVersion.R4)]
    [InlineData("r4b", FhirVersion.R4B)]
    [InlineData("R5", FhirVersion.R5)]
    [InlineData("nope", FhirVersion.Unknown)]
    public void ParseFromShortName_maps_expected(string input, FhirVersion expected)
        => FhirVersionParser.ParseFromShortName(input).Should().Be(expected);
}

public class FhirVersionDetectorTests
{
    [Theory]
    [InlineData("https://hapi.fhir.org/baseR4", FhirVersion.R4)]
    [InlineData("https://example.com/fhir/r4/metadata", FhirVersion.R4)]
    [InlineData("https://example.com/r4b/", FhirVersion.R4B)]
    [InlineData("https://example.com/R5/fhir", FhirVersion.R5)]
    [InlineData("https://server.fire.ly", FhirVersion.Unknown)]
    public void FromBaseUrl_detects_line_hint(string url, FhirVersion expected)
        => FhirVersionDetector.FromBaseUrl(url).Should().Be(expected);

    [Fact]
    public void TryParseFromMetadataJson_reads_fhirVersion_property()
    {
        const string json = """{"resourceType":"CapabilityStatement","fhirVersion":"4.0.1","rest":[]}""";
        FhirVersionDetector.TryParseFromMetadataJson(json).Should().Be(FhirVersion.R4);
    }

    [Fact]
    public void ResolveDeserializeVersion_prefers_json_then_url_then_declared()
    {
        FhirVersionDetector.ResolveDeserializeVersion(FhirVersion.R5, FhirVersion.R4, FhirVersion.R4B)
            .Should().Be(FhirVersion.R5);
        FhirVersionDetector.ResolveDeserializeVersion(FhirVersion.Unknown, FhirVersion.R4, FhirVersion.R4B)
            .Should().Be(FhirVersion.R4);
        FhirVersionDetector.ResolveDeserializeVersion(FhirVersion.Unknown, FhirVersion.Unknown, FhirVersion.R4B)
            .Should().Be(FhirVersion.R4B);
    }
}

public class FhirCapabilityRuntimeTests
{
    private readonly FhirCapabilityRuntime _runtime = new();

    [Fact]
    public void ParseMetadata_r5_minimal_roundtrip_model()
    {
        const string json =
            """{"resourceType":"CapabilityStatement","fhirVersion":"5.0.0","kind":"instance","software":{"name":"T"},"format":["json"],"rest":[{"mode":"server","resource":[{"type":"Patient","searchParam":[{"name":"_id","type":"token","documentation":"x"}]}]}]}""";
        var r = _runtime.ParseMetadata(json, "https://example.com", FhirVersion.R5, FhirVersionResolutionStrategy.PreferDetected);
        r.Model.ServerResources.Should().ContainSingle(x => x.Type == "Patient");
        r.Model.ServerResources[0].SearchParams.Should().Contain(x => x.Name == "_id");
    }
}
