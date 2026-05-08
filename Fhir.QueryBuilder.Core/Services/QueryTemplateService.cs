using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Fhir.QueryBuilder.Services
{
    public class QueryTemplateService : IQueryTemplateService
    {
        private readonly ILogger<QueryTemplateService> _logger;
        private readonly QueryBuilderAppSettings _options;
        private readonly string _templatesPath;
        private readonly string _historyPath;
        private readonly List<QueryTemplate> _templates = new();
        private readonly List<QueryHistory> _history = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public QueryTemplateService(
            ILogger<QueryTemplateService> logger,
            IOptions<QueryBuilderAppSettings> options)
        {
            _logger = logger;
            _options = options.Value;
            _templatesPath = Path.Combine(_options.QueryHistoryPath, "templates.json");
            _historyPath = Path.Combine(_options.QueryHistoryPath, "history.json");
            
            EnsureDirectoryExists();
            _ = Task.Run(LoadDataAsync);
        }

        public async Task<IEnumerable<QueryTemplate>> GetTemplatesAsync(string? resourceType = null, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var templates = _templates.AsEnumerable();
                
                if (!string.IsNullOrEmpty(resourceType))
                {
                    templates = templates.Where(t => t.ResourceType.Equals(resourceType, StringComparison.OrdinalIgnoreCase));
                }

                return templates.OrderByDescending(t => t.IsFavorite)
                               .ThenByDescending(t => t.UsageCount)
                               .ThenBy(t => t.Name)
                               .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<QueryTemplate?> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                return _templates.FirstOrDefault(t => t.Id == templateId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<QueryTemplate> SaveTemplateAsync(QueryTemplate template, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var existing = _templates.FirstOrDefault(t => t.Id == template.Id);
                if (existing != null)
                {
                    // Update existing template
                    existing.Name = template.Name;
                    existing.Description = template.Description;
                    existing.ResourceType = template.ResourceType;
                    existing.QueryPattern = template.QueryPattern;
                    existing.Parameters = template.Parameters;
                    existing.Tags = template.Tags;
                    existing.IsFavorite = template.IsFavorite;
                    
                    _logger.LogDebug("Updated template: {TemplateName}", template.Name);
                }
                else
                {
                    // Add new template
                    template.CreatedAt = DateTime.UtcNow;
                    _templates.Add(template);
                    
                    _logger.LogDebug("Added new template: {TemplateName}", template.Name);
                }

                await SaveTemplatesAsync();
                return template;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var template = _templates.FirstOrDefault(t => t.Id == templateId);
                if (template != null && !template.IsBuiltIn)
                {
                    _templates.Remove(template);
                    await SaveTemplatesAsync();
                    
                    _logger.LogDebug("Deleted template: {TemplateId}", templateId);
                    return true;
                }
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<QueryTemplate>> SearchTemplatesAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetTemplatesAsync(cancellationToken: cancellationToken);
                }

                var lowerSearchTerm = searchTerm.ToLowerInvariant();
                
                return _templates.Where(t => 
                    t.Name.ToLowerInvariant().Contains(lowerSearchTerm) ||
                    t.Description.ToLowerInvariant().Contains(lowerSearchTerm) ||
                    t.ResourceType.ToLowerInvariant().Contains(lowerSearchTerm) ||
                    t.Tags.Any(tag => tag.ToLowerInvariant().Contains(lowerSearchTerm)))
                    .OrderByDescending(t => t.IsFavorite)
                    .ThenByDescending(t => t.UsageCount)
                    .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<QueryTemplate>> GetFavoriteTemplatesAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                return _templates.Where(t => t.IsFavorite)
                                .OrderByDescending(t => t.UsageCount)
                                .ThenBy(t => t.Name)
                                .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> ToggleFavoriteAsync(string templateId, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var template = _templates.FirstOrDefault(t => t.Id == templateId);
                if (template != null)
                {
                    template.IsFavorite = !template.IsFavorite;
                    await SaveTemplatesAsync();
                    
                    _logger.LogDebug("Toggled favorite for template: {TemplateId} to {IsFavorite}", templateId, template.IsFavorite);
                    return true;
                }
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<QueryHistory>> GetQueryHistoryAsync(int limit = 50, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                return _history.OrderByDescending(h => h.ExecutedAt)
                              .Take(limit)
                              .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task AddToHistoryAsync(QueryHistory historyItem, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                _history.Add(historyItem);
                
                // Keep only the most recent items
                while (_history.Count > _options.MaxQueryHistoryItems)
                {
                    var oldest = _history.OrderBy(h => h.ExecutedAt).First();
                    _history.Remove(oldest);
                }

                await SaveHistoryAsync();
                _logger.LogDebug("Added query to history: {Query}", historyItem.Query);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ClearHistoryAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                _history.Clear();
                await SaveHistoryAsync();
                _logger.LogInformation("Cleared query history");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<QueryTemplate> CreateTemplateFromHistoryAsync(string historyId, string templateName, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var historyItem = _history.FirstOrDefault(h => h.Id == historyId);
                if (historyItem == null)
                {
                    throw new ArgumentException($"History item not found: {historyId}");
                }

                var template = new QueryTemplate
                {
                    Name = templateName,
                    Description = $"Created from query executed on {historyItem.ExecutedAt:yyyy-MM-dd HH:mm}",
                    ResourceType = historyItem.ResourceType,
                    QueryPattern = historyItem.Query,
                    Tags = new List<string> { "from-history" }
                };

                return await SaveTemplateAsync(template, cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<QueryTemplate>> GetBuiltInTemplatesAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                return _templates.Where(t => t.IsBuiltIn).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task InitializeBuiltInTemplatesAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                // Only add built-in templates if they don't exist
                if (_templates.Any(t => t.IsBuiltIn))
                {
                    return;
                }

                var builtInTemplates = GetDefaultTemplates();
                _templates.AddRange(builtInTemplates);
                
                await SaveTemplatesAsync();
                _logger.LogInformation("Initialized {Count} built-in templates", builtInTemplates.Count);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                await LoadTemplatesAsync();
                await LoadHistoryAsync();
                await InitializeBuiltInTemplatesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading template and history data");
            }
        }

        private async Task LoadTemplatesAsync()
        {
            if (File.Exists(_templatesPath))
            {
                var json = await File.ReadAllTextAsync(_templatesPath);
                var templates = JsonSerializer.Deserialize<List<QueryTemplate>>(json);
                if (templates != null)
                {
                    _templates.AddRange(templates);
                    _logger.LogDebug("Loaded {Count} templates", templates.Count);
                }
            }
        }

        private async Task LoadHistoryAsync()
        {
            if (File.Exists(_historyPath))
            {
                var json = await File.ReadAllTextAsync(_historyPath);
                var history = JsonSerializer.Deserialize<List<QueryHistory>>(json);
                if (history != null)
                {
                    _history.AddRange(history);
                    _logger.LogDebug("Loaded {Count} history items", history.Count);
                }
            }
        }

        private async Task SaveTemplatesAsync()
        {
            var json = JsonSerializer.Serialize(_templates, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_templatesPath, json);
        }

        private async Task SaveHistoryAsync()
        {
            var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_historyPath, json);
        }

        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(_templatesPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static List<QueryTemplate> GetDefaultTemplates()
        {
            return new List<QueryTemplate>
            {
                new()
                {
                    Name = "All Patients",
                    Description = "Retrieve all patients with basic information",
                    ResourceType = "Patient",
                    QueryPattern = "{baseUrl}/Patient",
                    IsBuiltIn = true,
                    Tags = new List<string> { "basic", "patient" }
                },
                new()
                {
                    Name = "Patient by Family Name",
                    Description = "Find patients by family name",
                    ResourceType = "Patient",
                    QueryPattern = "{baseUrl}/Patient?family={familyName}",
                    Parameters = new Dictionary<string, string> { { "familyName", "Smith" } },
                    IsBuiltIn = true,
                    Tags = new List<string> { "search", "patient", "name" }
                },
                new()
                {
                    Name = "Recent Observations",
                    Description = "Get observations from the last 30 days",
                    ResourceType = "Observation",
                    QueryPattern = "{baseUrl}/Observation?date=ge{date}",
                    Parameters = new Dictionary<string, string> { { "date", DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd") } },
                    IsBuiltIn = true,
                    Tags = new List<string> { "observation", "recent", "date" }
                },
                new()
                {
                    Name = "Patient with Organization",
                    Description = "Get patients with their managing organization",
                    ResourceType = "Patient",
                    QueryPattern = "{baseUrl}/Patient?_include=Patient:organization",
                    IsBuiltIn = true,
                    Tags = new List<string> { "patient", "organization", "include" }
                }
            };
        }
    }
}
