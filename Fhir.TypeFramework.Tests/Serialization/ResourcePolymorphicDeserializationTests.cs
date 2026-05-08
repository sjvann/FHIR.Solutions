using System.Text.Json;
using Fhir.Resources.R5;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Serialization;
using Xunit;

namespace Fhir.TypeFramework.Tests.Serialization;

public class ResourcePolymorphicDeserializationTests
{
    private static readonly IReadOnlyDictionary<string, Type> R5Map =
        FhirResourceTypeMap.FromResourceAssembly(typeof(Patient).Assembly, typeof(Resource));

    [Fact]
    public void DeserializeResource_PatientJson_ReturnsPatientInstance()
    {
        const string json = """{"resourceType":"Patient","id":"p1"}""";
        var r = FhirJsonSerializer.DeserializeResource(json, R5Map);
        Assert.NotNull(r);
        Assert.IsType<Patient>(r);
        Assert.Equal("p1", ((Patient)r).Id);
    }

    [Fact]
    public void Deserialize_BundleWithPatientEntry_ResolvesEntryResource()
    {
        const string json = """
            {
              "resourceType": "Bundle",
              "type": "searchset",
              "entry": [
                { "resource": { "resourceType": "Patient", "id": "x" } }
              ]
            }
            """;
        var opts = FhirJsonSerializer.OptionsWithPolymorphicResources(R5Map);
        var bundle = JsonSerializer.Deserialize<Bundle>(json, opts);
        Assert.NotNull(bundle);
        Assert.Single(bundle.Entry!);
        var res = bundle.Entry![0].Resource;
        Assert.NotNull(res);
        Assert.IsType<Patient>(res);
    }
}
