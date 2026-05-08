namespace Fhir.QueryBuilder.Services.Interfaces
{
    public class ProgressInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Operation { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int PercentComplete { get; set; }
        public bool IsIndeterminate { get; set; } = true;
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCancelled { get; set; }
        public Exception? Error { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
    }

    public interface IProgressService
    {
        string StartOperation(string operation, string message = "", bool isIndeterminate = true);
        void UpdateProgress(string operationId, string message, int percentComplete = 0);
        void UpdateProgress(string operationId, int percentComplete);
        void CompleteOperation(string operationId, string? finalMessage = null);
        void CancelOperation(string operationId, string? reason = null);
        void FailOperation(string operationId, Exception error, string? message = null);
        ProgressInfo? GetProgress(string operationId);
        IEnumerable<ProgressInfo> GetActiveOperations();
        void ClearCompletedOperations();
        
        event EventHandler<ProgressInfo>? ProgressChanged;
        event EventHandler<ProgressInfo>? OperationCompleted;
        event EventHandler<ProgressInfo>? OperationFailed;
    }
}
