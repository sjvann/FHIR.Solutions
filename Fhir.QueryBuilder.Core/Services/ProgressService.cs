using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Fhir.QueryBuilder.Services
{
    public class ProgressService : IProgressService
    {
        private readonly ILogger<ProgressService> _logger;
        private readonly ConcurrentDictionary<string, ProgressInfo> _operations = new();

        public event EventHandler<ProgressInfo>? ProgressChanged;
        public event EventHandler<ProgressInfo>? OperationCompleted;
        public event EventHandler<ProgressInfo>? OperationFailed;

        public ProgressService(ILogger<ProgressService> logger)
        {
            _logger = logger;
        }

        public string StartOperation(string operation, string message = "", bool isIndeterminate = true)
        {
            var progressInfo = new ProgressInfo
            {
                Operation = operation,
                Message = message,
                IsIndeterminate = isIndeterminate,
                PercentComplete = 0
            };

            _operations.TryAdd(progressInfo.Id, progressInfo);
            _logger.LogDebug("Started operation: {Operation} with ID: {Id}", operation, progressInfo.Id);
            
            ProgressChanged?.Invoke(this, progressInfo);
            return progressInfo.Id;
        }

        public void UpdateProgress(string operationId, string message, int percentComplete = 0)
        {
            if (_operations.TryGetValue(operationId, out var progressInfo))
            {
                progressInfo.Message = message;
                progressInfo.PercentComplete = Math.Clamp(percentComplete, 0, 100);
                progressInfo.IsIndeterminate = percentComplete == 0;

                _logger.LogDebug("Updated progress for {Operation}: {Message} ({PercentComplete}%)", 
                    progressInfo.Operation, message, percentComplete);
                
                ProgressChanged?.Invoke(this, progressInfo);
            }
        }

        public void UpdateProgress(string operationId, int percentComplete)
        {
            if (_operations.TryGetValue(operationId, out var progressInfo))
            {
                progressInfo.PercentComplete = Math.Clamp(percentComplete, 0, 100);
                progressInfo.IsIndeterminate = false;

                _logger.LogDebug("Updated progress for {Operation}: {PercentComplete}%", 
                    progressInfo.Operation, percentComplete);
                
                ProgressChanged?.Invoke(this, progressInfo);
            }
        }

        public void CompleteOperation(string operationId, string? finalMessage = null)
        {
            if (_operations.TryGetValue(operationId, out var progressInfo))
            {
                progressInfo.IsCompleted = true;
                progressInfo.EndTime = DateTime.UtcNow;
                progressInfo.PercentComplete = 100;
                progressInfo.IsIndeterminate = false;
                
                if (!string.IsNullOrEmpty(finalMessage))
                {
                    progressInfo.Message = finalMessage;
                }

                var duration = progressInfo.EndTime.Value - progressInfo.StartTime;
                _logger.LogInformation("Completed operation: {Operation} in {Duration}ms", 
                    progressInfo.Operation, duration.TotalMilliseconds);

                ProgressChanged?.Invoke(this, progressInfo);
                OperationCompleted?.Invoke(this, progressInfo);
            }
        }

        public void CancelOperation(string operationId, string? reason = null)
        {
            if (_operations.TryGetValue(operationId, out var progressInfo))
            {
                progressInfo.IsCancelled = true;
                progressInfo.EndTime = DateTime.UtcNow;
                
                if (!string.IsNullOrEmpty(reason))
                {
                    progressInfo.Message = reason;
                }

                _logger.LogInformation("Cancelled operation: {Operation} - {Reason}", 
                    progressInfo.Operation, reason ?? "User cancelled");
                
                OperationCompleted?.Invoke(this, progressInfo);
            }
        }

        public void FailOperation(string operationId, Exception error, string? message = null)
        {
            if (_operations.TryGetValue(operationId, out var progressInfo))
            {
                progressInfo.Error = error;
                progressInfo.EndTime = DateTime.UtcNow;
                
                if (!string.IsNullOrEmpty(message))
                {
                    progressInfo.Message = message;
                }
                else
                {
                    progressInfo.Message = error.Message;
                }

                _logger.LogError(error, "Failed operation: {Operation}", progressInfo.Operation);
                
                OperationFailed?.Invoke(this, progressInfo);
            }
        }

        public ProgressInfo? GetProgress(string operationId)
        {
            _operations.TryGetValue(operationId, out var progressInfo);
            return progressInfo;
        }

        public IEnumerable<ProgressInfo> GetActiveOperations()
        {
            return _operations.Values.Where(p => !p.IsCompleted && !p.IsCancelled && p.Error == null);
        }

        public void ClearCompletedOperations()
        {
            var completedIds = _operations.Values
                .Where(p => p.IsCompleted || p.IsCancelled || p.Error != null)
                .Select(p => p.Id)
                .ToList();

            foreach (var id in completedIds)
            {
                _operations.TryRemove(id, out _);
            }

            _logger.LogDebug("Cleared {Count} completed operations", completedIds.Count);
        }
    }
}
