namespace Fhir.QueryBuilder.Services.Interfaces
{
    public enum ErrorSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    public class ErrorInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public ErrorSeverity Severity { get; set; } = ErrorSeverity.Error;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Source { get; set; }
        public Exception? Exception { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
    }

    public interface IErrorHandlingService
    {
        void HandleError(Exception exception, string? source = null, Dictionary<string, object>? context = null);
        void HandleError(string message, ErrorSeverity severity = ErrorSeverity.Error, string? source = null, Dictionary<string, object>? context = null);
        void LogError(ErrorInfo errorInfo);
        IEnumerable<ErrorInfo> GetRecentErrors(int count = 10);
        void ClearErrors();
        event EventHandler<ErrorInfo>? ErrorOccurred;
    }
}
