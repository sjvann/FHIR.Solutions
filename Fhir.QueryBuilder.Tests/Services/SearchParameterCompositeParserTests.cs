using Fhir.QueryBuilder.Services;
using FluentAssertions;
using Xunit;

namespace Fhir.QueryBuilder.Tests.Services;

public class SearchParameterCompositeParserTests
{
    private const string SampleBundle = """
        {
          "resourceType": "Bundle",
          "entry": [{
            "resource": {
              "resourceType": "SearchParameter",
              "code": "combo",
              "component": [
                { "definition": "http://example.org/sp-token" },
                { "definition": "http://example.org/sp-qty" }
              ]
            }
          }]
        }
        """;

    [Fact]
    public void TryGetComponentDefinitionCanonicals_ReturnsTwoUrls()
    {
        var list = SearchParameterCompositeParser.TryGetComponentDefinitionCanonicals(SampleBundle);

        Assert.NotNull(list);
        list.Should().HaveCount(2);
        list[0].Should().Be("http://example.org/sp-token");
        list[1].Should().Be("http://example.org/sp-qty");
    }

    [Fact]
    public void TryGetFirstSearchParameterTypeFromBundle_ReadsType()
    {
        var bundle = """
            {"resourceType":"Bundle","entry":[{"resource":{"resourceType":"SearchParameter","type":"token"}}]}
            """;

        SearchParameterCompositeParser.TryGetFirstSearchParameterTypeFromBundle(bundle).Should().Be("token");
    }
}
