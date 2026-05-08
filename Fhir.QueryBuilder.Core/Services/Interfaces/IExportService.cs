namespace Fhir.QueryBuilder.Services.Interfaces
{
    public enum ExportFormat
    {
        Json,
        Xml,
        Csv,
        Excel,
        Pdf
    }

    public class ExportOptions
    {
        public ExportFormat Format { get; set; } = ExportFormat.Json;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public bool IncludeMetadata { get; set; } = true;
        public bool PrettyPrint { get; set; } = true;
        public bool CompressOutput { get; set; } = false;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    public class ExportResult
    {
        public bool IsSuccess { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public TimeSpan ExportDuration { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public interface IExportService
    {
        Task<ExportResult> ExportQueryResultAsync(
            string queryResult, 
            ExportOptions options, 
            CancellationToken cancellationToken = default);

        Task<ExportResult> ExportMultipleResultsAsync(
            IEnumerable<string> queryResults, 
            ExportOptions options, 
            CancellationToken cancellationToken = default);

        Task<ExportResult> ExportTemplatesAsync(
            IEnumerable<object> templates,
            ExportOptions options,
            CancellationToken cancellationToken = default);

        Task<ExportResult> ExportHistoryAsync(
            IEnumerable<object> history,
            ExportOptions options,
            CancellationToken cancellationToken = default);

        IEnumerable<ExportFormat> GetSupportedFormats();
        string GetDefaultFileName(ExportFormat format, string? prefix = null);
        Task<bool> ValidateExportOptionsAsync(ExportOptions options, CancellationToken cancellationToken = default);
    }
}
