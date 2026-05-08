namespace Fhir.QueryBuilder.Services.Interfaces
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
        public string? NextPageUrl { get; set; }
        public string? PreviousPageUrl { get; set; }
        public string? FirstPageUrl { get; set; }
        public string? LastPageUrl { get; set; }
    }

    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? BaseQuery { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }

    public interface IPaginationService
    {
        Task<PagedResult<T>> GetPagedResultAsync<T>(
            PaginationRequest request, 
            Func<string, CancellationToken, Task<string>> queryExecutor,
            Func<string, IEnumerable<T>> resultParser,
            CancellationToken cancellationToken = default);

        string BuildPagedQuery(PaginationRequest request);
        PaginationRequest ParseFhirBundleLinks(string bundleJson, PaginationRequest currentRequest);
        Task<IEnumerable<T>> GetAllPagesAsync<T>(
            PaginationRequest request,
            Func<string, CancellationToken, Task<string>> queryExecutor,
            Func<string, IEnumerable<T>> resultParser,
            int maxPages = 100,
            CancellationToken cancellationToken = default);
    }
}
