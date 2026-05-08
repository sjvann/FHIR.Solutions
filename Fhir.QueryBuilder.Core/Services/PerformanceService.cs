using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;

namespace Fhir.QueryBuilder.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly ILogger<PerformanceService> _logger;
        private readonly ConcurrentDictionary<string, PerformanceCounter> _counters = new();
        private readonly ConcurrentQueue<PerformanceMetrics> _recentMetrics = new();
        private const int MaxRecentMetrics = 1000;

        public event EventHandler<PerformanceMetrics>? MetricsRecorded;

        public PerformanceService(ILogger<PerformanceService> logger)
        {
            _logger = logger;
        }

        public PerformanceOperation StartOperation(string operationName)
        {
            return new PerformanceOperation(this, operationName);
        }

        public void RecordMetrics(PerformanceMetrics metrics)
        {
            try
            {
                // Update counter
                var counter = _counters.AddOrUpdate(metrics.OperationName, 
                    _ => new PerformanceCounter { Name = metrics.OperationName },
                    (_, existing) => existing);

                lock (counter)
                {
                    counter.TotalOperations++;
                    counter.TotalDuration = counter.TotalDuration.Add(metrics.Duration);

                    if (metrics.IsSuccess)
                    {
                        counter.SuccessfulOperations++;
                    }
                    else
                    {
                        counter.FailedOperations++;
                    }

                    if (metrics.Duration < counter.MinDuration)
                    {
                        counter.MinDuration = metrics.Duration;
                    }

                    if (metrics.Duration > counter.MaxDuration)
                    {
                        counter.MaxDuration = metrics.Duration;
                    }
                }

                // Store recent metrics
                _recentMetrics.Enqueue(metrics);
                while (_recentMetrics.Count > MaxRecentMetrics)
                {
                    _recentMetrics.TryDequeue(out _);
                }

                // Log performance data
                var logLevel = metrics.IsSuccess ? LogLevel.Debug : LogLevel.Warning;
                _logger.Log(logLevel, 
                    "Operation {OperationName} completed in {Duration}ms. Success: {IsSuccess}. Memory: {MemoryUsed}B",
                    metrics.OperationName, 
                    metrics.Duration.TotalMilliseconds,
                    metrics.IsSuccess,
                    metrics.MemoryUsedBytes);

                // Raise event
                MetricsRecorded?.Invoke(this, metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording performance metrics for operation: {OperationName}", metrics.OperationName);
            }
        }

        public PerformanceCounter GetCounter(string operationName)
        {
            return _counters.GetValueOrDefault(operationName) ?? new PerformanceCounter { Name = operationName };
        }

        public IEnumerable<PerformanceCounter> GetAllCounters()
        {
            return _counters.Values.ToList();
        }

        public void ResetCounters()
        {
            _counters.Clear();
            while (_recentMetrics.TryDequeue(out _)) { }
            _logger.LogInformation("Performance counters reset");
        }

        public async Task<string> GeneratePerformanceReportAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var report = new StringBuilder();
                report.AppendLine("=== FHIR Query Builder Performance Report ===");
                report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                report.AppendLine();

                // System information
                report.AppendLine("=== System Information ===");
                report.AppendLine($"Total Memory: {GC.GetTotalMemory(false):N0} bytes");
                report.AppendLine($"Working Set: {Environment.WorkingSet:N0} bytes");
                report.AppendLine($"Thread Pool Threads: {System.Threading.ThreadPool.ThreadCount}");
                report.AppendLine($"Processor Count: {Environment.ProcessorCount}");
                report.AppendLine();

                // Performance counters
                report.AppendLine("=== Operation Performance ===");
                var counters = GetAllCounters().OrderByDescending(c => c.TotalOperations);
                
                if (!counters.Any())
                {
                    report.AppendLine("No performance data available.");
                }
                else
                {
                    report.AppendLine($"{"Operation",-30} {"Count",-8} {"Success%",-8} {"Avg(ms)",-10} {"Min(ms)",-10} {"Max(ms)",-10}");
                    report.AppendLine(new string('-', 88));

                    foreach (var counter in counters)
                    {
                        report.AppendLine($"{counter.Name,-30} {counter.TotalOperations,-8} {counter.SuccessRate,-7:F1}% {counter.AverageDuration.TotalMilliseconds,-9:F1} {counter.MinDuration.TotalMilliseconds,-9:F1} {counter.MaxDuration.TotalMilliseconds,-9:F1}");
                    }
                }

                report.AppendLine();

                // Recent slow operations
                report.AppendLine("=== Recent Slow Operations (>1000ms) ===");
                var slowOperations = _recentMetrics
                    .Where(m => m.Duration.TotalMilliseconds > 1000)
                    .OrderByDescending(m => m.Duration)
                    .Take(10);

                if (!slowOperations.Any())
                {
                    report.AppendLine("No slow operations detected.");
                }
                else
                {
                    foreach (var op in slowOperations)
                    {
                        report.AppendLine($"{op.StartTime:HH:mm:ss} - {op.OperationName}: {op.Duration.TotalMilliseconds:F0}ms");
                        if (!string.IsNullOrEmpty(op.ErrorMessage))
                        {
                            report.AppendLine($"  Error: {op.ErrorMessage}");
                        }
                    }
                }

                report.AppendLine();

                // Memory usage patterns
                report.AppendLine("=== Memory Usage Patterns ===");
                var memoryIntensiveOps = _recentMetrics
                    .Where(m => m.MemoryUsedBytes > 1024 * 1024) // > 1MB
                    .GroupBy(m => m.OperationName)
                    .Select(g => new
                    {
                        Operation = g.Key,
                        Count = g.Count(),
                        AvgMemory = g.Average(m => m.MemoryUsedBytes),
                        MaxMemory = g.Max(m => m.MemoryUsedBytes)
                    })
                    .OrderByDescending(x => x.AvgMemory);

                if (!memoryIntensiveOps.Any())
                {
                    report.AppendLine("No memory-intensive operations detected.");
                }
                else
                {
                    report.AppendLine($"{"Operation",-30} {"Count",-8} {"Avg Memory",-15} {"Max Memory",-15}");
                    report.AppendLine(new string('-', 70));

                    foreach (var op in memoryIntensiveOps)
                    {
                        report.AppendLine($"{op.Operation,-30} {op.Count,-8} {op.AvgMemory / 1024 / 1024,-14:F1}MB {op.MaxMemory / 1024 / 1024,-14:F1}MB");
                    }
                }

                return report.ToString();
            }, cancellationToken);
        }
    }
}
