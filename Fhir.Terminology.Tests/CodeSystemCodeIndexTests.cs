using Fhir.Terminology.Core;

namespace Fhir.Terminology.Tests;

public class CodeSystemCodeIndexTests
{
    private const string TreeJson = """
        {
          "resourceType": "CodeSystem",
          "url": "http://example.org/cs",
          "concept": [
            { "code": "a", "display": "A", "concept": [
              { "code": "b", "display": "B" }
            ]}
          ]
        }
        """;

    [Fact]
    public void Subsumes_ancestor_relationship()
    {
        var o = CodeSystemCodeIndex.ComputeSubsumes(TreeJson, "a", "b");
        Assert.Equal("subsumes", o);
    }

    [Fact]
    public void Subsumes_equivalent()
    {
        var o = CodeSystemCodeIndex.ComputeSubsumes(TreeJson, "b", "b");
        Assert.Equal("equivalent", o);
    }
}
