using Fhir.Terminology.Infrastructure;

namespace Fhir.Terminology.Tests;

public class ThoCatalogUrlsTests
{
    [Fact]
    public void Browse_pages_match_tho_public_catalog()
    {
        Assert.Equal("https://terminology.hl7.org/en/codesystems.html", ThoCatalogUrls.CodeSystemsBrowse);
        Assert.Equal("https://terminology.hl7.org/en/valuesets.html", ThoCatalogUrls.ValueSetsBrowse);
        Assert.Equal("https://terminology.hl7.org/en/external_code_systems.html", ThoCatalogUrls.ExternalCodeSystemsBrowse);
    }
}
