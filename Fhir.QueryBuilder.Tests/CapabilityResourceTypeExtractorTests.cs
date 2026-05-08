using Fhir.QueryBuilder.Metadata;

namespace Fhir.QueryBuilder.Tests;

public sealed class CapabilityResourceTypeExtractorTests
{
    [Fact]
    public void FromCapabilityJson_parses_rest_resource_types()
    {
        const string json = """
            {"resourceType":"CapabilityStatement","rest":[{"mode":"server","resource":[{"type":"Observation"},{"type":"Patient"}]}]}
            """;

        var types = CapabilityResourceTypeExtractor.FromCapabilityJson(json);

        Assert.NotNull(types);
        Assert.Contains("Patient", types);
        Assert.Contains("Observation", types);
        Assert.Equal(types.OrderBy(x => x).ToList(), types);
    }

    [Fact]
    public void FromCapabilityJson_returns_null_for_invalid_json()
    {
        Assert.Null(CapabilityResourceTypeExtractor.FromCapabilityJson("{"));
    }
}
