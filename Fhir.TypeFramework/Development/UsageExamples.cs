using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Fhir.TypeFramework.Performance;

namespace Fhir.TypeFramework.Development;

public static class UsageExamples
{
    public static void BasicTypeExamples()
    {
        var name = new FhirString("John Doe");
        var uri = new FhirUri("https://example.com");
        var id = new FhirId("patient-123");

        _ = (name, uri, id);
    }

    public static void PerformanceExamples()
    {
        PerformanceMonitor.EnableMonitoring = true;
        DeepCopyOptimizer.EnableOptimization = true;

        using var measurement = PerformanceMonitor.Measure("Create items");
        var items = new List<FhirString> { new("a"), new("b"), new("c") };
        _ = (measurement, items);
    }
}

