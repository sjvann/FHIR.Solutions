using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Fhir.QueryBuilder.Services
{
    public class ErrorHandlingService : IErrorHandlingService
    {
        private readonly ILogger<ErrorHandlingService> _logger;
        private readonly ConcurrentQueue<ErrorInfo> _recentErrors = new();
        private const int MaxRecentErrors = 100;

        public event EventHandler<ErrorInfo>? ErrorOccurred;

        public ErrorHandlingService(ILogger<ErrorHandlingService> logger)
        {
            _logger = logger;
        }

        public void HandleError(Exception exception, string? source = null, Dictionary<string, object>? context = null)
        {
            var errorInfo = new ErrorInfo
            {
                Message = exception.Message,
                Details = exception.ToString(),
                Severity = GetSeverityFromException(exception),
                Source = source ?? exception.Source,
                Exception = exception,
                Context = context ?? new Dictionary<string, object>()
            };

            LogError(errorInfo);
        }

        public void HandleError(string message, ErrorSeverity severity = ErrorSeverity.Error, string? source = null, Dictionary<string, object>? context = null)
        {
            var errorInfo = new ErrorInfo
            {
                Message = message,
                Severity = severity,
                Source = source,
                Context = context ?? new Dictionary<string, object>()
            };

            LogError(errorInfo);
        }

        public void LogError(ErrorInfo errorInfo)
        {
            // Add to recent errors queue
            _recentErrors.Enqueue(errorInfo);
            
            // Maintain queue size
            while (_recentErrors.Count > MaxRecentErrors)
            {
                _recentErrors.TryDequeue(out _);
            }

            // Log using Microsoft.Extensions.Logging
            var logLevel = GetLogLevel(errorInfo.Severity);
            _logger.Log(logLevel, errorInfo.Exception, 
                "Error from {Source}: {Message}", 
                errorInfo.Source ?? "Unknown", 
                errorInfo.Message);

            // Raise event
            ErrorOccurred?.Invoke(this, errorInfo);
        }

        public IEnumerable<ErrorInfo> GetRecentErrors(int count = 10)
        {
            return _recentErrors.TakeLast(count).Reverse();
        }

        public void ClearErrors()
        {
            while (_recentErrors.TryDequeue(out _)) { }
            _logger.LogInformation("Error history cleared");
        }

        private static ErrorSeverity GetSeverityFromException(Exception exception)
        {
            return exception switch
            {
                ArgumentException => ErrorSeverity.Warning,
                InvalidOperationException => ErrorSeverity.Error,
                NotSupportedException => ErrorSeverity.Warning,
                TimeoutException => ErrorSeverity.Warning,
                UnauthorizedAccessException => ErrorSeverity.Error,
                System.Net.Http.HttpRequestException => ErrorSeverity.Error,
                _ => ErrorSeverity.Error
            };
        }

        private static LogLevel GetLogLevel(ErrorSeverity severity)
        {
            return severity switch
            {
                ErrorSeverity.Info => LogLevel.Information,
                ErrorSeverity.Warning => LogLevel.Warning,
                ErrorSeverity.Error => LogLevel.Error,
                ErrorSeverity.Critical => LogLevel.Critical,
                _ => LogLevel.Error
            };
        }
    }
}
