using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Fhir.QueryBuilder.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<ConfigurationService> _logger;
        private readonly IOptionsMonitor<QueryBuilderAppSettings> _optionsMonitor;
        private readonly string _configFilePath;
        private readonly string _backupDirectory;

        public event EventHandler<QueryBuilderAppSettings>? ConfigurationChanged;

        public ConfigurationService(
            ILogger<ConfigurationService> logger,
            IOptionsMonitor<QueryBuilderAppSettings> optionsMonitor)
        {
            _logger = logger;
            _optionsMonitor = optionsMonitor;
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            _backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigBackups");
            
            // Subscribe to configuration changes
            _optionsMonitor.OnChange(OnConfigurationChanged);
        }

        public async Task<QueryBuilderAppSettings> LoadConfigurationAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Loading configuration from: {ConfigFilePath}", _configFilePath);

                if (!File.Exists(_configFilePath))
                {
                    _logger.LogWarning("Configuration file not found, using defaults");
                    return new QueryBuilderAppSettings();
                }

                var jsonContent = await File.ReadAllTextAsync(_configFilePath, cancellationToken);
                var configRoot = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                
                if (configRoot?.TryGetValue(QueryBuilderAppSettings.SectionName, out var sectionValue) == true)
                {
                    var sectionJson = JsonSerializer.Serialize(sectionValue);
                    var options = JsonSerializer.Deserialize<QueryBuilderAppSettings>(sectionJson);
                    
                    _logger.LogInformation("Configuration loaded successfully");
                    return options ?? new QueryBuilderAppSettings();
                }

                _logger.LogWarning("Configuration section not found, using defaults");
                return new QueryBuilderAppSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load configuration");
                throw;
            }
        }

        public async Task SaveConfigurationAsync(QueryBuilderAppSettings options, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Saving configuration to: {ConfigFilePath}", _configFilePath);

                // Validate configuration before saving
                var validationResult = await ValidateConfigurationAsync(options, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new InvalidOperationException($"Configuration validation failed: {string.Join(", ", validationResult.Errors)}");
                }

                // Create backup before saving
                await CreateBackupAsync(cancellationToken);

                // Load existing configuration
                var existingConfig = new Dictionary<string, object>();
                if (File.Exists(_configFilePath))
                {
                    var existingContent = await File.ReadAllTextAsync(_configFilePath, cancellationToken);
                    existingConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(existingContent) ?? new();
                }

                // Update the FHIR section
                existingConfig[QueryBuilderAppSettings.SectionName] = options;

                // Save updated configuration
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var updatedContent = JsonSerializer.Serialize(existingConfig, jsonOptions);
                await File.WriteAllTextAsync(_configFilePath, updatedContent, cancellationToken);

                _logger.LogInformation("Configuration saved successfully");
                ConfigurationChanged?.Invoke(this, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save configuration");
                throw;
            }
        }

        public async Task<Fhir.QueryBuilder.Services.Interfaces.ValidationResult> ValidateConfigurationAsync(QueryBuilderAppSettings options, CancellationToken cancellationToken = default)
        {
            var result = new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true };

            try
            {
                // Use data annotations validation
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(options);
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                
                if (!Validator.TryValidateObject(options, validationContext, validationResults, true))
                {
                    foreach (var validationError in validationResults)
                    {
                        result.AddError(validationError.ErrorMessage ?? "Unknown validation error");
                    }
                }

                // Custom validation rules
                if (!Uri.TryCreate(options.DefaultServerUrl, UriKind.Absolute, out var uri))
                {
                    result.AddError("Default server URL is not a valid absolute URI");
                }
                else if (uri.Scheme != "https" && uri.Scheme != "http")
                {
                    result.AddError("Default server URL must use HTTP or HTTPS protocol");
                }

                // Validate UI settings
                var supportedThemes = new[] { "Light", "Dark", "Auto" };
                if (!supportedThemes.Contains(options.Ui.Theme))
                {
                    result.AddWarning($"Unsupported theme '{options.Ui.Theme}'. Supported themes: {string.Join(", ", supportedThemes)}");
                }

                // Validate export settings
                if (!Directory.Exists(Path.GetDirectoryName(options.Export.DefaultExportPath)))
                {
                    result.AddWarning("Export directory does not exist and will be created");
                }

                // Validate security settings
                if (options.Security.RequireHttps && options.DefaultServerUrl.StartsWith("http://"))
                {
                    result.AddWarning("HTTPS is required but default server URL uses HTTP");
                }

                _logger.LogDebug("Configuration validation completed: {IsValid}", result.IsValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during configuration validation");
                result.AddError($"Validation error: {ex.Message}");
            }

            return result;
        }

        public async Task ResetToDefaultsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Resetting configuration to defaults");
                
                var defaultOptions = new QueryBuilderAppSettings();
                await SaveConfigurationAsync(defaultOptions, cancellationToken);
                
                _logger.LogInformation("Configuration reset to defaults successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset configuration to defaults");
                throw;
            }
        }

        public async Task<Dictionary<string, object>> GetEnvironmentVariablesAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var envVars = new Dictionary<string, object>();
                
                foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
                {
                    var key = envVar.Key?.ToString();
                    var value = envVar.Value?.ToString();
                    
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("FHIR_", StringComparison.OrdinalIgnoreCase))
                    {
                        envVars[key] = value ?? string.Empty;
                    }
                }
                
                _logger.LogDebug("Retrieved {Count} FHIR-related environment variables", envVars.Count);
                return envVars;
            }, cancellationToken);
        }

        public async Task<bool> BackupConfigurationAsync(string backupPath, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    _logger.LogWarning("Configuration file does not exist, cannot create backup");
                    return false;
                }

                var backupDir = Path.GetDirectoryName(backupPath);
                if (!string.IsNullOrEmpty(backupDir) && !Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                await File.WriteAllBytesAsync(backupPath, await File.ReadAllBytesAsync(_configFilePath, cancellationToken), cancellationToken);
                
                _logger.LogInformation("Configuration backed up to: {BackupPath}", backupPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to backup configuration to: {BackupPath}", backupPath);
                return false;
            }
        }

        public async Task<bool> RestoreConfigurationAsync(string backupPath, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    _logger.LogError("Backup file does not exist: {BackupPath}", backupPath);
                    return false;
                }

                // Validate backup file before restoring
                var backupContent = await File.ReadAllTextAsync(backupPath, cancellationToken);
                var backupConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(backupContent);
                
                if (backupConfig?.ContainsKey(QueryBuilderAppSettings.SectionName) != true)
                {
                    _logger.LogError("Backup file does not contain valid configuration");
                    return false;
                }

                await File.WriteAllTextAsync(_configFilePath, backupContent, cancellationToken);
                
                _logger.LogInformation("Configuration restored from: {BackupPath}", backupPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore configuration from: {BackupPath}", backupPath);
                return false;
            }
        }

        private void OnConfigurationChanged(QueryBuilderAppSettings options, string? name)
        {
            _logger.LogInformation("Configuration changed: {Name}", name ?? "Unknown");
            ConfigurationChanged?.Invoke(this, options);
        }

        private async Task CreateBackupAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!Directory.Exists(_backupDirectory))
                {
                    Directory.CreateDirectory(_backupDirectory);
                }

                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var backupFileName = $"appsettings_backup_{timestamp}.json";
                var backupPath = Path.Combine(_backupDirectory, backupFileName);

                await BackupConfigurationAsync(backupPath, cancellationToken);

                // Clean up old backups (keep only last 10)
                var backupFiles = Directory.GetFiles(_backupDirectory, "appsettings_backup_*.json")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .Skip(10);

                foreach (var oldBackup in backupFiles)
                {
                    File.Delete(oldBackup);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create automatic backup");
            }
        }
    }
}
