using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Fhir.TypeFramework.Abstractions;

namespace Fhir.TypeFramework.Performance;

public static class ValidationOptimizer
{
    public static bool EnableOptimization { get; set; } = false;

    private static readonly ConcurrentDictionary<string, Regex> _regexCache = new(StringComparer.Ordinal);

    public static Regex GetCachedRegex(string pattern, RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant)
        => _regexCache.GetOrAdd(pattern, p => new Regex(p, options));

    public static IEnumerable<ValidationResult> BatchValidate(IEnumerable<ITypeFramework> items, ValidationContext? context = null)
    {
        if (!EnableOptimization)
        {
            foreach (var item in items)
            {
                if (item is IValidatableObject validatable)
                {
                    foreach (var r in validatable.Validate(context ?? new ValidationContext(item)))
                        yield return r;
                }
            }
            yield break;
        }

        using var _ = PerformanceMonitor.Measure("BatchValidate");

        foreach (var item in items)
        {
            if (item is IValidatableObject validatable)
            {
                foreach (var r in validatable.Validate(context ?? new ValidationContext(item)))
                    yield return r;
            }
        }
    }

    public static bool BatchQuickValidate(IEnumerable<ITypeFramework> items)
    {
        using var _ = PerformanceMonitor.Measure("BatchQuickValidate");

        foreach (var item in items)
        {
            if (item is IValidatableObject validatable)
            {
                var ctx = new ValidationContext(item);
                if (validatable.Validate(ctx).Any())
                    return false;
            }
        }
        return true;
    }

    public static void ClearRegexCache() => _regexCache.Clear();
}

