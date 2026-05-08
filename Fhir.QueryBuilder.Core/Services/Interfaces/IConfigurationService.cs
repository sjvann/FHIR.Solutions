using Fhir.QueryBuilder.Configuration;

namespace Fhir.QueryBuilder.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<QueryBuilderAppSettings> LoadConfigurationAsync(CancellationToken cancellationToken = default);
        Task SaveConfigurationAsync(QueryBuilderAppSettings options, CancellationToken cancellationToken = default);
        Task<ValidationResult> ValidateConfigurationAsync(QueryBuilderAppSettings options, CancellationToken cancellationToken = default);
        Task ResetToDefaultsAsync(CancellationToken cancellationToken = default);
        Task<Dictionary<string, object>> GetEnvironmentVariablesAsync(CancellationToken cancellationToken = default);
        Task<bool> BackupConfigurationAsync(string backupPath, CancellationToken cancellationToken = default);
        Task<bool> RestoreConfigurationAsync(string backupPath, CancellationToken cancellationToken = default);
        
        event EventHandler<QueryBuilderAppSettings>? ConfigurationChanged;
    }
}
