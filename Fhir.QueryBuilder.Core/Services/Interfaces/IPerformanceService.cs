namespace Fhir.QueryBuilder.Services.Interfaces
{
    public class PerformanceMetrics
    {
        public string OperationName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public long MemoryUsedBytes { get; set; }
        public int ThreadCount { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    public class PerformanceCounter
    {
        public string Name { get; set; } = string.Empty;
        public long TotalOperations { get; set; }
        public long SuccessfulOperations { get; set; }
        public long FailedOperations { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public TimeSpan AverageDuration => TotalOperations > 0 ? TimeSpan.FromTicks(TotalDuration.Ticks / TotalOperations) : TimeSpan.Zero;
        public TimeSpan MinDuration { get; set; } = TimeSpan.MaxValue;
        public TimeSpan MaxDuration { get; set; } = TimeSpan.MinValue;
        public double SuccessRate => TotalOperations > 0 ? (double)SuccessfulOperations / TotalOperations * 100 : 0;
    }

    public interface IPerformanceService
    {
        PerformanceOperation StartOperation(string operationName);
        void RecordMetrics(PerformanceMetrics metrics);
        PerformanceCounter GetCounter(string operationName);
        IEnumerable<PerformanceCounter> GetAllCounters();
        void ResetCounters();
        Task<string> GeneratePerformanceReportAsync(CancellationToken cancellationToken = default);
        
        event EventHandler<PerformanceMetrics>? MetricsRecorded;
    }

    public class PerformanceOperation : IDisposable
    {
        private readonly IPerformanceService _performanceService;
        private readonly PerformanceMetrics _metrics;
        private readonly long _startMemory;
        private readonly int _startThreadCount;
        private bool _disposed;

        public PerformanceOperation(IPerformanceService performanceService, string operationName)
        {
            _performanceService = performanceService;
            _metrics = new PerformanceMetrics
            {
                OperationName = operationName,
                StartTime = DateTime.UtcNow
            };
            
            _startMemory = GC.GetTotalMemory(false);
            _startThreadCount = System.Threading.ThreadPool.ThreadCount;
        }

        public void AddCustomMetric(string key, object value)
        {
            _metrics.CustomMetrics[key] = value;
        }

        public void MarkAsFailure(string errorMessage)
        {
            _metrics.IsSuccess = false;
            _metrics.ErrorMessage = errorMessage;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _metrics.EndTime = DateTime.UtcNow;
            _metrics.MemoryUsedBytes = GC.GetTotalMemory(false) - _startMemory;
            _metrics.ThreadCount = System.Threading.ThreadPool.ThreadCount - _startThreadCount;

            _performanceService.RecordMetrics(_metrics);
            _disposed = true;
        }
    }
}
