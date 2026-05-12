using System.Text.Json;
using Fhir.Terminology.Core;

namespace Fhir.Terminology.Tests;

public class TerminologyImportJsonExtractorTests
{
    [Fact]
    public void ExtractImportablePieces_bundle_keeps_codesystem_and_valueset_in_same_file()
    {
        var json =
            """
            {
              "resourceType": "Bundle",
              "type": "collection",
              "entry": [
                {
                  "resource": {
                    "resourceType": "CodeSystem",
                    "id": "example-cs",
                    "url": "http://example/cs",
                    "status": "active",
                    "content": "complete"
                  }
                },
                {
                  "resource": {
                    "resourceType": "ValueSet",
                    "id": "example-vs",
                    "url": "http://example/vs",
                    "status": "active"
                  }
                }
              ]
            }
            """;

        var pieces = TerminologyImportJsonExtractor.ExtractImportablePieces(json, out var skipReason);

        Assert.Equal(2, pieces.Count);
        Assert.Null(skipReason);

        using (var doc = JsonDocument.Parse(pieces[0]))
        {
            Assert.Equal("CodeSystem", doc.RootElement.GetProperty("resourceType").GetString());
            Assert.Equal("example-cs", doc.RootElement.GetProperty("id").GetString());
        }

        using (var doc = JsonDocument.Parse(pieces[1]))
        {
            Assert.Equal("ValueSet", doc.RootElement.GetProperty("resourceType").GetString());
            Assert.Equal("example-vs", doc.RootElement.GetProperty("id").GetString());
        }
    }
}
