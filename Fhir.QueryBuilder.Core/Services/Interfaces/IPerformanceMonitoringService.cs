using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.Services.Interfaces
{
    /// <summary>
    /// 效能監控服務介面
    /// </summary>
    public interface IPerformanceMonitoringService
    {
        /// <summary>
        /// 開始監控操作
        /// </summary>
        /// <param name="operationName">操作名稱</param>
        /// <param name="metadata">元數據</param>
        /// <returns>操作 ID</returns>
        string StartOperation(string operationName, Dictionary<string, object>? metadata = null);

        /// <summary>
        /// 結束監控操作
        /// </summary>
        /// <param name="operationId">操作 ID</param>
        /// <param name="success">是否成功</param>
        /// <param name="errorMessage">錯誤訊息</param>
        void EndOperation(string operationId, bool success = true, string? errorMessage = null);

        /// <summary>
        /// 記錄指標
        /// </summary>
        /// <param name="metricName">指標名稱</param>
        /// <param name="value">值</param>
        /// <param name="tags">標籤</param>
        void RecordMetric(string metricName, double value, Dictionary<string, string>? tags = null);

        /// <summary>
        /// 取得效能統計
        /// </summary>
        /// <param name="operationName">操作名稱（可選）</param>
        /// <param name="timeRange">時間範圍</param>
        /// <returns>效能統計</returns>
        Task<PerformanceStatistics> GetStatisticsAsync(string? operationName = null, TimeSpan? timeRange = null);

        /// <summary>
        /// 清除統計資料
        /// </summary>
        /// <param name="olderThan">清除早於指定時間的資料</param>
        Task ClearStatisticsAsync(DateTime? olderThan = null);
    }

    /// <summary>
    /// 效能統計
    /// </summary>
    public class PerformanceStatistics
    {
        /// <summary>
        /// 操作名稱
        /// </summary>
        public string OperationName { get; set; } = string.Empty;

        /// <summary>
        /// 總執行次數
        /// </summary>
        public long TotalExecutions { get; set; }

        /// <summary>
        /// 成功次數
        /// </summary>
        public long SuccessfulExecutions { get; set; }

        /// <summary>
        /// 失敗次數
        /// </summary>
        public long FailedExecutions { get; set; }

        /// <summary>
        /// 成功率
        /// </summary>
        public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions : 0;

        /// <summary>
        /// 平均執行時間（毫秒）
        /// </summary>
        public double AverageExecutionTimeMs { get; set; }

        /// <summary>
        /// 最小執行時間（毫秒）
        /// </summary>
        public double MinExecutionTimeMs { get; set; }

        /// <summary>
        /// 最大執行時間（毫秒）
        /// </summary>
        public double MaxExecutionTimeMs { get; set; }

        /// <summary>
        /// 第 95 百分位執行時間（毫秒）
        /// </summary>
        public double P95ExecutionTimeMs { get; set; }

        /// <summary>
        /// 第 99 百分位執行時間（毫秒）
        /// </summary>
        public double P99ExecutionTimeMs { get; set; }

        /// <summary>
        /// 統計時間範圍開始
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 統計時間範圍結束
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 最近的錯誤訊息
        /// </summary>
        public List<string> RecentErrors { get; set; } = new();
    }

    /// <summary>
    /// 效能監控服務實作
    /// </summary>
    public class PerformanceMonitoringService : IPerformanceMonitoringService
    {
        private readonly ConcurrentDictionary<string, OperationContext> _activeOperations = new();
        private readonly ConcurrentDictionary<string, List<OperationRecord>> _operationHistory = new();
        private readonly ConcurrentDictionary<string, List<MetricRecord>> _metrics = new();
        private readonly ILogger<PerformanceMonitoringService> _logger;

        public PerformanceMonitoringService(ILogger<PerformanceMonitoringService> logger)
        {
            _logger = logger;
        }

        public string StartOperation(string operationName, Dictionary<string, object>? metadata = null)
        {
            var operationId = Guid.NewGuid().ToString();
            var context = new OperationContext
            {
                OperationId = operationId,
                OperationName = operationName,
                StartTime = DateTime.UtcNow,
                Metadata = metadata ?? new Dictionary<string, object>()
            };

            _activeOperations[operationId] = context;
            _logger.LogDebug("Started monitoring operation: {OperationName} ({OperationId})", operationName, operationId);

            return operationId;
        }

        public void EndOperation(string operationId, bool success = true, string? errorMessage = null)
        {
            if (!_activeOperations.TryRemove(operationId, out var context))
            {
                _logger.LogWarning("Attempted to end unknown operation: {OperationId}", operationId);
                return;
            }

            var endTime = DateTime.UtcNow;
            var duration = endTime - context.StartTime;

            var record = new OperationRecord
            {
                OperationName = context.OperationName,
                StartTime = context.StartTime,
                EndTime = endTime,
                DurationMs = duration.TotalMilliseconds,
                Success = success,
                ErrorMessage = errorMessage,
                Metadata = context.Metadata
            };

            // 儲存操作記錄
            _operationHistory.AddOrUpdate(
                context.OperationName,
                new List<OperationRecord> { record },
                (key, existing) =>
                {
                    existing.Add(record);
                    // 保持最近 1000 筆記錄
                    if (existing.Count > 1000)
                    {
                        existing.RemoveRange(0, existing.Count - 1000);
                    }
                    return existing;
                });

            _logger.LogDebug("Ended monitoring operation: {OperationName} ({OperationId}), Duration: {Duration}ms, Success: {Success}",
                context.OperationName, operationId, duration.TotalMilliseconds, success);

            if (!success && !string.IsNullOrEmpty(errorMessage))
            {
                _logger.LogWarning("Operation failed: {OperationName} ({OperationId}), Error: {Error}",
                    context.OperationName, operationId, errorMessage);
            }
        }

        public void RecordMetric(string metricName, double value, Dictionary<string, string>? tags = null)
        {
            var record = new MetricRecord
            {
                MetricName = metricName,
                Value = value,
                Timestamp = DateTime.UtcNow,
                Tags = tags ?? new Dictionary<string, string>()
            };

            _metrics.AddOrUpdate(
                metricName,
                new List<MetricRecord> { record },
                (key, existing) =>
                {
                    existing.Add(record);
                    // 保持最近 1000 筆記錄
                    if (existing.Count > 1000)
                    {
                        existing.RemoveRange(0, existing.Count - 1000);
                    }
                    return existing;
                });

            _logger.LogDebug("Recorded metric: {MetricName} = {Value}", metricName, value);
        }

        public async Task<PerformanceStatistics> GetStatisticsAsync(string? operationName = null, TimeSpan? timeRange = null)
        {
            var cutoffTime = timeRange.HasValue ? DateTime.UtcNow - timeRange.Value : DateTime.MinValue;

            if (!string.IsNullOrEmpty(operationName))
            {
                return await GetOperationStatisticsAsync(operationName, cutoffTime);
            }

            // 如果沒有指定操作名稱，返回所有操作的統計
            var allStats = new List<PerformanceStatistics>();
            foreach (var kvp in _operationHistory)
            {
                var stats = await GetOperationStatisticsAsync(kvp.Key, cutoffTime);
                allStats.Add(stats);
            }

            // 合併統計（簡化實作）
            if (allStats.Any())
            {
                return allStats.OrderByDescending(s => s.TotalExecutions).First();
            }

            return new PerformanceStatistics();
        }

        public async Task ClearStatisticsAsync(DateTime? olderThan = null)
        {
            var cutoffTime = olderThan ?? DateTime.UtcNow;

            foreach (var kvp in _operationHistory.ToList())
            {
                var filteredRecords = kvp.Value.Where(r => r.StartTime >= cutoffTime).ToList();
                if (filteredRecords.Any())
                {
                    _operationHistory[kvp.Key] = filteredRecords;
                }
                else
                {
                    _operationHistory.TryRemove(kvp.Key, out _);
                }
            }

            foreach (var kvp in _metrics.ToList())
            {
                var filteredMetrics = kvp.Value.Where(m => m.Timestamp >= cutoffTime).ToList();
                if (filteredMetrics.Any())
                {
                    _metrics[kvp.Key] = filteredMetrics;
                }
                else
                {
                    _metrics.TryRemove(kvp.Key, out _);
                }
            }

            _logger.LogInformation("Cleared performance statistics older than {CutoffTime}", cutoffTime);
            await Task.CompletedTask;
        }

        private async Task<PerformanceStatistics> GetOperationStatisticsAsync(string operationName, DateTime cutoffTime)
        {
            if (!_operationHistory.TryGetValue(operationName, out var records))
            {
                return new PerformanceStatistics { OperationName = operationName };
            }

            var filteredRecords = records.Where(r => r.StartTime >= cutoffTime).ToList();
            if (!filteredRecords.Any())
            {
                return new PerformanceStatistics { OperationName = operationName };
            }

            var durations = filteredRecords.Select(r => r.DurationMs).OrderBy(d => d).ToList();
            var successfulRecords = filteredRecords.Where(r => r.Success).ToList();
            var failedRecords = filteredRecords.Where(r => !r.Success).ToList();

            var statistics = new PerformanceStatistics
            {
                OperationName = operationName,
                TotalExecutions = filteredRecords.Count,
                SuccessfulExecutions = successfulRecords.Count,
                FailedExecutions = failedRecords.Count,
                AverageExecutionTimeMs = durations.Average(),
                MinExecutionTimeMs = durations.Min(),
                MaxExecutionTimeMs = durations.Max(),
                P95ExecutionTimeMs = GetPercentile(durations, 0.95),
                P99ExecutionTimeMs = GetPercentile(durations, 0.99),
                StartTime = filteredRecords.Min(r => r.StartTime),
                EndTime = filteredRecords.Max(r => r.EndTime),
                RecentErrors = failedRecords
                    .Where(r => !string.IsNullOrEmpty(r.ErrorMessage))
                    .OrderByDescending(r => r.StartTime)
                    .Take(10)
                    .Select(r => r.ErrorMessage!)
                    .ToList()
            };

            return await Task.FromResult(statistics);
        }

        private double GetPercentile(List<double> sortedValues, double percentile)
        {
            if (!sortedValues.Any()) return 0;

            var index = (int)Math.Ceiling(sortedValues.Count * percentile) - 1;
            index = Math.Max(0, Math.Min(index, sortedValues.Count - 1));
            return sortedValues[index];
        }

        private class OperationContext
        {
            public string OperationId { get; set; } = string.Empty;
            public string OperationName { get; set; } = string.Empty;
            public DateTime StartTime { get; set; }
            public Dictionary<string, object> Metadata { get; set; } = new();
        }

        private class OperationRecord
        {
            public string OperationName { get; set; } = string.Empty;
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public double DurationMs { get; set; }
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
            public Dictionary<string, object> Metadata { get; set; } = new();
        }

        private class MetricRecord
        {
            public string MetricName { get; set; } = string.Empty;
            public double Value { get; set; }
            public DateTime Timestamp { get; set; }
            public Dictionary<string, string> Tags { get; set; } = new();
        }
    }
}
