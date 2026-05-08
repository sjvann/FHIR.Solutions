using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Fhir.Terminology.Tests;

public class RemoteTerminologyForwardTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RemoteTerminologyForwardTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private HttpClient CreateClient(string dbPath)
    {
        return _factory.WithWebHostBuilder(b =>
        {
            b.UseSetting("ConnectionStrings:Terminology", $"Data Source={dbPath}");
            b.UseSetting("Terminology:SeedDemoResources", "false");
            b.UseSetting("Terminology:SeedExternalCanonicalCodeSystems", "false");
            b.UseSetting("Terminology:SeedInternalFhirCodeSystems", "false");
        }).CreateClient();
    }

    [Fact]
    public async Task ValueSet_expand_by_url_delegates_to_configured_upstream()
    {
        using var server = WireMockServer.Start();

        server
            .Given(
                Request.Create()
                    .UsingGet()
                    .WithPath("/fhir/ValueSet/$expand")
                    .WithParam("url", "http://remote/vs"))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/fhir+json")
                    .WithBody(
                        """
                        {"resourceType":"ValueSet","url":"http://remote/vs","status":"active","expansion":{"total":1,"contains":[{"system":"http://x","code":"mock","display":"Mock"}]}}
                        """));

        var db = Path.Combine(Path.GetTempPath(), $"fhir-term-wm-{Guid.NewGuid():n}.db");
        if (File.Exists(db))
            File.Delete(db);

        using var client = CreateClient(db);

        var upstream = new
        {
            systemUriPrefix = "http",
            baseUrl = $"{server.Url}/fhir",
            timeoutSeconds = 30,
        };
        var postUp = await client.PostAsync(
            "/fhir/admin/registry/upstreams",
            new StringContent(JsonSerializer.Serialize(upstream), Encoding.UTF8, "application/json"));
        postUp.EnsureSuccessStatusCode();

        var expand = await client.GetAsync("/fhir/ValueSet/$expand?url=http%3A%2F%2Fremote%2Fvs");
        var body = await expand.Content.ReadAsStringAsync();
        Assert.True(expand.IsSuccessStatusCode, body);
        Assert.Contains("mock", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Validate_code_uses_binding_registry_profile_and_path()
    {
        var db = Path.Combine(Path.GetTempPath(), $"fhir-term-bind-{Guid.NewGuid():n}.db");
        if (File.Exists(db))
            File.Delete(db);

        using var client = CreateClient(db);

        var cs = """
            {"resourceType":"CodeSystem","id":"cs1","url":"http://example/cs","status":"active","content":"complete","concept":[{"code":"a","display":"A"}]}
            """;
        Assert.True((await client.PostAsync("/fhir/CodeSystem", new StringContent(cs, Encoding.UTF8, "application/fhir+json"))).IsSuccessStatusCode);

        var vs = """
            {"resourceType":"ValueSet","id":"vs1","url":"http://example/vs","status":"active","compose":{"include":[{"system":"http://example/cs"}]}}
            """;
        Assert.True((await client.PostAsync("/fhir/ValueSet", new StringContent(vs, Encoding.UTF8, "application/fhir+json"))).IsSuccessStatusCode);

        var bind = new
        {
            structureDefinitionUrl = "http://hl7.org/fhir/StructureDefinition/Patient",
            elementPath = "Patient.gender",
            valueSetCanonical = "http://example/vs",
            strength = "required",
        };
        Assert.True((await client.PostAsync(
            "/fhir/admin/registry/bindings",
            new StringContent(JsonSerializer.Serialize(bind), Encoding.UTF8, "application/json"))).IsSuccessStatusCode);

        var validate = await client.GetAsync(
            "/fhir/ValueSet/$validate-code?profile=http%3A%2F%2Fhl7.org%2Ffhir%2FStructureDefinition%2FPatient&path=Patient.gender&code=a&system=http%3A%2F%2Fexample%2Fcs");
        var body = await validate.Content.ReadAsStringAsync();
        Assert.True(validate.IsSuccessStatusCode, body);
        using var doc = JsonDocument.Parse(body);
        var ok = false;
        foreach (var p in doc.RootElement.GetProperty("parameter").EnumerateArray())
        {
            if (p.GetProperty("name").GetString() == "result" &&
                p.TryGetProperty("valueBoolean", out var vb) &&
                vb.GetBoolean())
            {
                ok = true;
                break;
            }
        }

        Assert.True(ok, body);
    }
}
