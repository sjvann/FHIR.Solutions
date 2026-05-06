using System.Collections.Concurrent;

namespace Fhir.TypeFramework.Performance;

public static class TypeFrameworkCache
{
    public static bool EnableCaching { get; set; } = false;

    private static readonly ConcurrentDictionary<string, object?> _cache = new(StringComparer.Ordinal);

    public static T? GetOrAdd<T>(string key, Func<T?> factory)
    {
        if (!EnableCaching) return factory();
        return (T?)_cache.GetOrAdd(key, _ => factory());
    }

    public static bool TryGet<T>(string key, out T? value)
    {
        if (!EnableCaching)
        {
            value = default;
            return false;
        }

        if (_cache.TryGetValue(key, out var obj) && obj is T typed)
        {
            value = typed;
            return true;
        }

        value = default;
        return false;
    }

    public static void Set<T>(string key, T? value)
    {
        if (!EnableCaching) return;
        _cache[key] = value;
    }

    public static void ClearAll() => _cache.Clear();
}

