using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace Fhir.QueryBuilder.Services
{
    public class ExportService : IExportService
    {
        private readonly ILogger<ExportService> _logger;
        private readonly QueryBuilderAppSettings _options;

        public ExportService(
            ILogger<ExportService> logger,
            IOptions<QueryBuilderAppSettings> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<ExportResult> ExportQueryResultAsync(
            string queryResult, 
            ExportOptions options, 
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                _logger.LogDebug("Starting export to {Format} format", options.Format);

                if (!await ValidateExportOptionsAsync(options, cancellationToken))
                {
                    return new ExportResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid export options"
                    };
                }

                EnsureDirectoryExists(options.FilePath);

                var exportedContent = options.Format switch
                {
                    ExportFormat.Json => await ExportToJsonAsync(queryResult, options, cancellationToken),
                    ExportFormat.Xml => await ExportToXmlAsync(queryResult, options, cancellationToken),
                    ExportFormat.Csv => await ExportToCsvAsync(queryResult, options, cancellationToken),
                    _ => throw new NotSupportedException($"Export format {options.Format} is not supported")
                };

                var finalPath = options.FilePath;
                if (options.CompressOutput)
                {
                    finalPath = await CompressFileAsync(options.FilePath, cancellationToken);
                }

                var fileInfo = new FileInfo(finalPath);
                stopwatch.Stop();

                var result = new ExportResult
                {
                    IsSuccess = true,
                    FilePath = finalPath,
                    FileSizeBytes = fileInfo.Length,
                    ExportDuration = stopwatch.Elapsed,
                    Metadata = new Dictionary<string, object>
                    {
                        { "Format", options.Format.ToString() },
                        { "Compressed", options.CompressOutput },
                        { "IncludeMetadata", options.IncludeMetadata },
                        { "ExportedAt", DateTime.UtcNow }
                    }
                };

                _logger.LogInformation("Export completed successfully: {FilePath} ({FileSize} bytes)", 
                    finalPath, fileInfo.Length);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Export failed");
                
                return new ExportResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    ExportDuration = stopwatch.Elapsed
                };
            }
        }

        public async Task<ExportResult> ExportMultipleResultsAsync(
            IEnumerable<string> queryResults, 
            ExportOptions options, 
            CancellationToken cancellationToken = default)
        {
            var combinedResult = new StringBuilder();
            var resultCount = 0;

            if (options.Format == ExportFormat.Json)
            {
                combinedResult.AppendLine("[");
            }

            foreach (var result in queryResults)
            {
                if (resultCount > 0 && options.Format == ExportFormat.Json)
                {
                    combinedResult.AppendLine(",");
                }

                combinedResult.AppendLine(result);
                resultCount++;
            }

            if (options.Format == ExportFormat.Json)
            {
                combinedResult.AppendLine("]");
            }

            var exportOptions = new ExportOptions
            {
                Format = options.Format,
                FileName = options.FileName,
                FilePath = options.FilePath,
                IncludeMetadata = options.IncludeMetadata,
                PrettyPrint = options.PrettyPrint,
                CompressOutput = options.CompressOutput,
                CustomOptions = new Dictionary<string, object>(options.CustomOptions)
                {
                    { "ResultCount", resultCount }
                }
            };

            return await ExportQueryResultAsync(combinedResult.ToString(), exportOptions, cancellationToken);
        }

        public async Task<ExportResult> ExportTemplatesAsync(
            IEnumerable<object> templates, 
            ExportOptions options, 
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions 
            { 
                WriteIndented = options.PrettyPrint 
            });

            return await ExportQueryResultAsync(json, options, cancellationToken);
        }

        public async Task<ExportResult> ExportHistoryAsync(
            IEnumerable<object> history, 
            ExportOptions options, 
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(history, new JsonSerializerOptions 
            { 
                WriteIndented = options.PrettyPrint 
            });

            return await ExportQueryResultAsync(json, options, cancellationToken);
        }

        public IEnumerable<ExportFormat> GetSupportedFormats()
        {
            return new[] { ExportFormat.Json, ExportFormat.Xml, ExportFormat.Csv };
        }

        public string GetDefaultFileName(ExportFormat format, string? prefix = null)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var filePrefix = string.IsNullOrEmpty(prefix) ? "fhir_export" : prefix;
            var extension = format switch
            {
                ExportFormat.Json => "json",
                ExportFormat.Xml => "xml",
                ExportFormat.Csv => "csv",
                ExportFormat.Excel => "xlsx",
                ExportFormat.Pdf => "pdf",
                _ => "txt"
            };

            return $"{filePrefix}_{timestamp}.{extension}";
        }

        public async Task<bool> ValidateExportOptionsAsync(ExportOptions options, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(options.FileName))
            {
                options.FileName = GetDefaultFileName(options.Format);
            }

            if (string.IsNullOrEmpty(options.FilePath))
            {
                options.FilePath = Path.Combine(_options.Export.DefaultExportPath, options.FileName);
            }

            if (!GetSupportedFormats().Contains(options.Format))
            {
                _logger.LogError("Unsupported export format: {Format}", options.Format);
                return false;
            }

            // Check file size limits
            var directory = Path.GetDirectoryName(options.FilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                var driveInfo = new DriveInfo(Path.GetPathRoot(directory) ?? "C:");
                if (driveInfo.AvailableFreeSpace < _options.Export.MaxExportSizeMB * 1024 * 1024)
                {
                    _logger.LogError("Insufficient disk space for export");
                    return false;
                }
            }

            return true;
        }

        private async Task<string> ExportToJsonAsync(string content, ExportOptions options, CancellationToken cancellationToken)
        {
            var jsonContent = content;

            if (options.PrettyPrint)
            {
                try
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
                    jsonContent = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });
                }
                catch (JsonException)
                {
                    // If it's not valid JSON, keep original content
                    _logger.LogWarning("Content is not valid JSON, exporting as-is");
                }
            }

            if (options.IncludeMetadata)
            {
                var metadata = new
                {
                    ExportedAt = DateTime.UtcNow.ToString(_options.Export.DateTimeFormat),
                    Format = "JSON",
                    Generator = "Fhir.QueryBuilder",
                    Data = JsonSerializer.Deserialize<object>(jsonContent)
                };

                jsonContent = JsonSerializer.Serialize(metadata, new JsonSerializerOptions 
                { 
                    WriteIndented = options.PrettyPrint 
                });
            }

            await File.WriteAllTextAsync(options.FilePath, jsonContent, Encoding.UTF8, cancellationToken);
            return jsonContent;
        }

        private async Task<string> ExportToXmlAsync(string content, ExportOptions options, CancellationToken cancellationToken)
        {
            var xmlContent = content;

            // If content is JSON, try to convert to XML
            if (content.TrimStart().StartsWith("{") || content.TrimStart().StartsWith("["))
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(content);
                    xmlContent = ConvertJsonToXml(jsonDoc.RootElement);
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Failed to convert JSON to XML, using original content");
                }
            }

            if (options.IncludeMetadata)
            {
                var xmlDoc = new XmlDocument();
                var root = xmlDoc.CreateElement("FHIRExport");
                xmlDoc.AppendChild(root);

                var metadata = xmlDoc.CreateElement("Metadata");
                metadata.SetAttribute("ExportedAt", DateTime.UtcNow.ToString(_options.Export.DateTimeFormat));
                metadata.SetAttribute("Format", "XML");
                metadata.SetAttribute("Generator", "Fhir.QueryBuilder");
                root.AppendChild(metadata);

                var dataElement = xmlDoc.CreateElement("Data");
                dataElement.InnerXml = xmlContent;
                root.AppendChild(dataElement);

                xmlContent = xmlDoc.OuterXml;
            }

            if (options.PrettyPrint)
            {
                xmlContent = FormatXml(xmlContent);
            }

            await File.WriteAllTextAsync(options.FilePath, xmlContent, Encoding.UTF8, cancellationToken);
            return xmlContent;
        }

        private async Task<string> ExportToCsvAsync(string content, ExportOptions options, CancellationToken cancellationToken)
        {
            var csvContent = new StringBuilder();

            try
            {
                var jsonDoc = JsonDocument.Parse(content);
                csvContent = ConvertJsonToCsv(jsonDoc.RootElement);
            }
            catch (JsonException)
            {
                // If not JSON, treat as plain text
                csvContent.AppendLine("Content");
                csvContent.AppendLine($"\"{content.Replace("\"", "\"\"")}\"");
            }

            await File.WriteAllTextAsync(options.FilePath, csvContent.ToString(), Encoding.UTF8, cancellationToken);
            return csvContent.ToString();
        }

        private async Task<string> CompressFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var compressedPath = filePath + ".gz";
            
            using var originalFileStream = File.OpenRead(filePath);
            using var compressedFileStream = File.Create(compressedPath);
            using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
            
            await originalFileStream.CopyToAsync(compressionStream, cancellationToken);
            
            // Delete original file
            File.Delete(filePath);
            
            return compressedPath;
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static string ConvertJsonToXml(JsonElement element)
        {
            // Simplified JSON to XML conversion
            // In a real implementation, you might want to use a more sophisticated library
            return $"<root>{element}</root>";
        }

        private static string FormatXml(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                
                using var stringWriter = new StringWriter();
                using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings 
                { 
                    Indent = true, 
                    IndentChars = "  " 
                });
                
                doc.Save(xmlWriter);
                return stringWriter.ToString();
            }
            catch
            {
                return xml;
            }
        }

        private static StringBuilder ConvertJsonToCsv(JsonElement element)
        {
            var csv = new StringBuilder();
            
            // Simplified JSON to CSV conversion
            // This is a basic implementation - a real one would be more sophisticated
            if (element.ValueKind == JsonValueKind.Array)
            {
                var first = true;
                foreach (var item in element.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        if (first)
                        {
                            // Add headers
                            var headers = item.EnumerateObject().Select(p => p.Name);
                            csv.AppendLine(string.Join(",", headers));
                            first = false;
                        }
                        
                        // Add values
                        var values = item.EnumerateObject().Select(p => $"\"{p.Value}\"");
                        csv.AppendLine(string.Join(",", values));
                    }
                }
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                // Single object
                var headers = element.EnumerateObject().Select(p => p.Name);
                csv.AppendLine(string.Join(",", headers));
                
                var values = element.EnumerateObject().Select(p => $"\"{p.Value}\"");
                csv.AppendLine(string.Join(",", values));
            }
            
            return csv;
        }
    }
}
