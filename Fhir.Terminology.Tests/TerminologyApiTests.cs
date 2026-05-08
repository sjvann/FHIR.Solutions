using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fhir.Terminology.Tests;

public class TerminologyApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TerminologyApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private HttpClient CreateClient()
    {
        var db = Path.Combine(Path.GetTempPath(), $"fhir-term-{Guid.NewGuid():n}.db");
        if (File.Exists(db))
            File.Delete(db);
        return _factory.WithWebHostBuilder(b =>
        {
            b.UseSetting("ConnectionStrings:Terminology", $"Data Source={db}");
            b.UseSetting("Terminology:SeedDemoResources", "false");
            b.UseSetting("Terminology:SeedExternalCanonicalCodeSystems", "false");
            b.UseSetting("Terminology:SeedInternalFhirCodeSystems", "false");
        }).CreateClient();
    }

    [Fact]
    public async Task Metadata_returns_capability_statement()
    {
        using var client = CreateClient();
        var r = await client.GetAsync("/fhir/metadata");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var body = await r.Content.ReadAsStringAsync();
        Assert.Contains("CapabilityStatement", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Metadata_mode_terminology_returns_terminology_capabilities()
    {
        using var client = CreateClient();
        var r = await client.GetAsync("/fhir/metadata?mode=terminology");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var body = await r.Content.ReadAsStringAsync();
        Assert.Contains("TerminologyCapabilities", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Expand_valueset_after_seed()
    {
        using var client = CreateClient();
        var cs = """
            {"resourceType":"CodeSystem","id":"cs1","url":"http://hl7.org/fhir/administrative-gender","version":"4.0.1","status":"active","content":"complete","concept":[{"code":"male","display":"Male"},{"code":"female","display":"Female"}]}
            """;
        var postCs = await client.PostAsync("/fhir/CodeSystem", new StringContent(cs, System.Text.Encoding.UTF8, "application/fhir+json"));
        var postCsBody = await postCs.Content.ReadAsStringAsync();
        Assert.True(postCs.IsSuccessStatusCode, postCsBody);

        var vs = """
            {"resourceType":"ValueSet","id":"vs1","url":"http://example.org/vs/gender","status":"active","compose":{"include":[{"system":"http://hl7.org/fhir/administrative-gender","concept":[{"code":"male"}]}]}}
            """;
        var postVs = await client.PostAsync("/fhir/ValueSet", new StringContent(vs, System.Text.Encoding.UTF8, "application/fhir+json"));
        postVs.EnsureSuccessStatusCode();

        var expand = await client.GetAsync("/fhir/ValueSet/vs1/$expand");
        expand.EnsureSuccessStatusCode();
        var json = await expand.Content.ReadAsStringAsync();
        Assert.Contains("male", json, StringComparison.OrdinalIgnoreCase);
    }
}
