using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Fhir.QueryBuilder.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly ILogger<PaginationService> _logger;
        private readonly QueryBuilderAppSettings _options;
        private static readonly Regex UrlParameterRegex = new(@"[?&]_count=(\d+)", RegexOptions.Compiled);
        private static readonly Regex OffsetParameterRegex = new(@"[?&]_offset=(\d+)", RegexOptions.Compiled);

        public PaginationService(
            ILogger<PaginationService> logger,
            IOptions<QueryBuilderAppSettings> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<PagedResult<T>> GetPagedResultAsync<T>(
            PaginationRequest request,
            Func<string, CancellationToken, Task<string>> queryExecutor,
            Func<string, IEnumerable<T>> resultParser,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = BuildPagedQuery(request);
                _logger.LogDebug("Executing paged query: {Query}", query);

                var result = await queryExecutor(query, cancellationToken);
                var items = resultParser(result);

                var pagedResult = new PagedResult<T>
                {
                    Items = items,
                    CurrentPage = request.Page,
                    PageSize = request.PageSize
                };

                // Parse FHIR Bundle for pagination info
                var paginationInfo = ParseFhirBundleLinks(result, request);
                pagedResult.NextPageUrl = paginationInfo.Parameters.GetValueOrDefault("next");
                pagedResult.PreviousPageUrl = paginationInfo.Parameters.GetValueOrDefault("previous");
                pagedResult.FirstPageUrl = paginationInfo.Parameters.GetValueOrDefault("first");
                pagedResult.LastPageUrl = paginationInfo.Parameters.GetValueOrDefault("last");

                // Try to determine total count from Bundle
                if (TryGetTotalFromBundle(result, out var total))
                {
                    pagedResult.TotalItems = total;
                }
                else
                {
                    // Estimate based on current page
                    pagedResult.TotalItems = pagedResult.HasNextPage ? 
                        (request.Page * request.PageSize) + 1 : 
                        ((request.Page - 1) * request.PageSize) + items.Count();
                }

                _logger.LogDebug("Paged result: Page {Page}/{TotalPages}, Items: {ItemCount}/{TotalItems}",
                    pagedResult.CurrentPage, pagedResult.TotalPages, items.Count(), pagedResult.TotalItems);

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged result for page {Page}", request.Page);
                throw;
            }
        }

        public string BuildPagedQuery(PaginationRequest request)
        {
            if (string.IsNullOrEmpty(request.BaseQuery))
            {
                throw new ArgumentException("Base query cannot be empty", nameof(request));
            }

            var query = request.BaseQuery;
            var separator = query.Contains('?') ? "&" : "?";

            // Add or update _count parameter
            if (UrlParameterRegex.IsMatch(query))
            {
                query = UrlParameterRegex.Replace(query, $"_count={request.PageSize}");
            }
            else
            {
                query += $"{separator}_count={request.PageSize}";
                separator = "&";
            }

            // Add or update _offset parameter for pagination
            var offset = (request.Page - 1) * request.PageSize;
            if (offset > 0)
            {
                if (OffsetParameterRegex.IsMatch(query))
                {
                    query = OffsetParameterRegex.Replace(query, $"_offset={offset}");
                }
                else
                {
                    query += $"{separator}_offset={offset}";
                }
            }

            // Add any additional parameters
            foreach (var param in request.Parameters)
            {
                if (!query.Contains($"{param.Key}="))
                {
                    query += $"{separator}{param.Key}={Uri.EscapeDataString(param.Value)}";
                    separator = "&";
                }
            }

            return query;
        }

        public PaginationRequest ParseFhirBundleLinks(string bundleJson, PaginationRequest currentRequest)
        {
            var result = new PaginationRequest
            {
                Page = currentRequest.Page,
                PageSize = currentRequest.PageSize,
                BaseQuery = currentRequest.BaseQuery
            };

            try
            {
                using var document = JsonDocument.Parse(bundleJson);
                var root = document.RootElement;

                if (root.TryGetProperty("link", out var linksElement) && linksElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var link in linksElement.EnumerateArray())
                    {
                        if (link.TryGetProperty("relation", out var relationElement) &&
                            link.TryGetProperty("url", out var urlElement))
                        {
                            var relation = relationElement.GetString();
                            var url = urlElement.GetString();

                            if (!string.IsNullOrEmpty(relation) && !string.IsNullOrEmpty(url))
                            {
                                result.Parameters[relation] = url;
                            }
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse FHIR Bundle links");
            }

            return result;
        }

        public async Task<IEnumerable<T>> GetAllPagesAsync<T>(
            PaginationRequest request,
            Func<string, CancellationToken, Task<string>> queryExecutor,
            Func<string, IEnumerable<T>> resultParser,
            int maxPages = 100,
            CancellationToken cancellationToken = default)
        {
            var allItems = new List<T>();
            var currentRequest = new PaginationRequest
            {
                Page = 1,
                PageSize = Math.Min(request.PageSize, _options.Performance.MaxResultsPerPage),
                BaseQuery = request.BaseQuery,
                Parameters = new Dictionary<string, string>(request.Parameters)
            };

            var pageCount = 0;

            try
            {
                while (pageCount < maxPages && !cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Fetching page {Page} of all results", currentRequest.Page);

                    var pagedResult = await GetPagedResultAsync(currentRequest, queryExecutor, resultParser, cancellationToken);
                    allItems.AddRange(pagedResult.Items);

                    pageCount++;

                    if (!pagedResult.HasNextPage || string.IsNullOrEmpty(pagedResult.NextPageUrl))
                    {
                        break;
                    }

                    // Update request for next page
                    currentRequest.Page++;
                    
                    // If we have a next page URL, use it directly
                    if (!string.IsNullOrEmpty(pagedResult.NextPageUrl))
                    {
                        currentRequest.BaseQuery = pagedResult.NextPageUrl;
                        currentRequest.Parameters.Clear(); // URL already contains parameters
                    }
                }

                _logger.LogInformation("Retrieved {TotalItems} items across {PageCount} pages", allItems.Count, pageCount);
                return allItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all pages. Retrieved {ItemCount} items from {PageCount} pages", 
                    allItems.Count, pageCount);
                throw;
            }
        }

        private static bool TryGetTotalFromBundle(string bundleJson, out int total)
        {
            total = 0;

            try
            {
                using var document = JsonDocument.Parse(bundleJson);
                var root = document.RootElement;

                if (root.TryGetProperty("total", out var totalElement) && 
                    totalElement.ValueKind == JsonValueKind.Number)
                {
                    total = totalElement.GetInt32();
                    return true;
                }
            }
            catch (JsonException)
            {
                // Ignore JSON parsing errors
            }

            return false;
        }
    }
}
