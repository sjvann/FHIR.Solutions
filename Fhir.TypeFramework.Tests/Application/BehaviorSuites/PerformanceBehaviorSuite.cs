using System.ComponentModel.DataAnnotations;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Fhir.TypeFramework.Performance;
using Fhir.TypeFramework.Tests.Application.Contracts;

namespace Fhir.TypeFramework.Tests.Application.BehaviorSuites;

public sealed class PerformanceBehaviorSuite : IPerformanceBehaviorSuite
{
    public void VerifyDeepCopyOptimizerAndMonitoring()
    {
        PerformanceMonitor.EnableMonitoring = true;
        DeepCopyOptimizer.EnableOptimization = true;

        var list = new List<FhirString> { new("a"), new("b") };
        var copy = DeepCopyOptimizer.OptimizedDeepCopyList(list);
        Assert.NotNull(copy);
        Assert.Equal(list.Count, copy!.Count);
    }

    public void VerifyValidationOptimizerBatchApis()
    {
        ValidationOptimizer.EnableOptimization = true;

        var items = new List<ITypeFramework>
        {
            new FhirId("patient-123"),
            new FhirId("bad id")
        };

        var results = ValidationOptimizer.BatchValidate(items).ToList();
        Assert.NotEmpty(results);

        Assert.False(ValidationOptimizer.BatchQuickValidate(items));
    }

    public void VerifyTypeFrameworkCacheGetOrAddAndClear()
    {
        TypeFrameworkCache.EnableCaching = true;
        var x = TypeFrameworkCache.GetOrAdd("k2", () => 999);
        Assert.Equal(999, x);
        TypeFrameworkCache.ClearAll();
        Assert.False(TypeFrameworkCache.TryGet<int>("k2", out _));
    }
}
