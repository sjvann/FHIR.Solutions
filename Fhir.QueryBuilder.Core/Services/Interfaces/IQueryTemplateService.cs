namespace Fhir.QueryBuilder.Services.Interfaces
{
    public class QueryTemplate
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string QueryPattern { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUsed { get; set; } = DateTime.UtcNow;
        public int UsageCount { get; set; }
        public bool IsBuiltIn { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class QueryHistory
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Query { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan ExecutionTime { get; set; }
        public int ResultCount { get; set; }
        public bool IsSuccessful { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public interface IQueryTemplateService
    {
        Task<IEnumerable<QueryTemplate>> GetTemplatesAsync(string? resourceType = null, CancellationToken cancellationToken = default);
        Task<QueryTemplate?> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default);
        Task<QueryTemplate> SaveTemplateAsync(QueryTemplate template, CancellationToken cancellationToken = default);
        Task<bool> DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default);
        Task<IEnumerable<QueryTemplate>> SearchTemplatesAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<IEnumerable<QueryTemplate>> GetFavoriteTemplatesAsync(CancellationToken cancellationToken = default);
        Task<bool> ToggleFavoriteAsync(string templateId, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<QueryHistory>> GetQueryHistoryAsync(int limit = 50, CancellationToken cancellationToken = default);
        Task AddToHistoryAsync(QueryHistory historyItem, CancellationToken cancellationToken = default);
        Task ClearHistoryAsync(CancellationToken cancellationToken = default);
        Task<QueryTemplate> CreateTemplateFromHistoryAsync(string historyId, string templateName, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<QueryTemplate>> GetBuiltInTemplatesAsync(CancellationToken cancellationToken = default);
        Task InitializeBuiltInTemplatesAsync(CancellationToken cancellationToken = default);
    }
}
