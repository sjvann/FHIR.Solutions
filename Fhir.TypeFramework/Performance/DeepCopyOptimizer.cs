using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.Performance;

public static class DeepCopyOptimizer
{
    public static bool EnableOptimization { get; set; } = false;

    public static IList<T>? OptimizedDeepCopyList<T>(IList<T>? source) where T : Base
    {
        if (source == null) return null;
        if (!EnableOptimization)
        {
            return source.Select(item => (item.DeepCopy() as T)!).ToList();
        }

        using var _ = PerformanceMonitor.Measure($"DeepCopyList<{typeof(T).Name}>");

        if (source.Count == 0) return Array.Empty<T>();

        var result = new List<T>(source.Count);
        for (int i = 0; i < source.Count; i++)
        {
            result.Add((source[i].DeepCopy() as T)!);
        }

        return result;
    }
}

