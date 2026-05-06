using System.Collections.Concurrent;
using System.Diagnostics;

namespace Fhir.TypeFramework.Performance;

public static class PerformanceMonitor
{
    public static bool EnableMonitoring { get; set; } = false;

    private sealed class Metric
    {
        public long Count;
        public long TotalElapsedTicks;
        public long MinTicks = long.MaxValue;
        public long MaxTicks = 0;
    }

    private static readonly ConcurrentDictionary<string, Metric> _metrics = new(StringComparer.Ordinal);

    public readonly struct Measurement : IDisposable
    {
        private readonly string _name;
        private readonly long _startTimestamp;

        internal Measurement(string name)
        {
            _name = name;
            _startTimestamp = Stopwatch.GetTimestamp();
        }

        public void Dispose()
        {
            if (!EnableMonitoring) return;

            var elapsedTicks = Stopwatch.GetTimestamp() - _startTimestamp;
            var metric = _metrics.GetOrAdd(_name, _ => new Metric());

            Interlocked.Increment(ref metric.Count);
            Interlocked.Add(ref metric.TotalElapsedTicks, elapsedTicks);

            long current;
            while (elapsedTicks < (current = Volatile.Read(ref metric.MinTicks)))
            {
                if (Interlocked.CompareExchange(ref metric.MinTicks, elapsedTicks, current) == current) break;
            }

            while (elapsedTicks > (current = Volatile.Read(ref metric.MaxTicks)))
            {
                if (Interlocked.CompareExchange(ref metric.MaxTicks, elapsedTicks, current) == current) break;
            }
        }
    }

    public static Measurement Measure(string name)
    {
        if (!EnableMonitoring) return default;
        return new Measurement(name);
    }

    public sealed record PerformanceReport(long TotalOperations, double TotalElapsedMilliseconds, double AverageOperationMilliseconds);

    public static PerformanceReport GenerateReport()
    {
        long totalOps = 0;
        long totalTicks = 0;

        foreach (var kvp in _metrics)
        {
            totalOps += Volatile.Read(ref kvp.Value.Count);
            totalTicks += Volatile.Read(ref kvp.Value.TotalElapsedTicks);
        }

        var totalMs = TicksToMilliseconds(totalTicks);
        var avgMs = totalOps == 0 ? 0 : totalMs / totalOps;
        return new PerformanceReport(totalOps, totalMs, avgMs);
    }

    public static void ClearMetrics() => _metrics.Clear();

    private static double TicksToMilliseconds(long stopwatchTicks)
        => stopwatchTicks * 1000.0 / Stopwatch.Frequency;
}

