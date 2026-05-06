using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.Extensions;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.Examples;

public static class ValidationExample
{
    public static void Run()
    {
        var id = new FhirId("patient-123");
        var ok = ValidationFramework.ValidateFhirId(id.StringValue);

        var element = new Extension { Url = "http://example.com/ext" };
        element.CreateExtension("http://example.com/custom", "customValue");

        _ = ok;
    }
}

