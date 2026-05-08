namespace Fhir.QueryBuilder.Services.Interfaces
{
    public class ConnectionPoolStats
    {
        public int TotalConnections { get; set; }
        public int ActiveConnections { get; set; }
        public int IdleConnections { get; set; }
        public int PendingRequests { get; set; }
        public TimeSpan AverageConnectionTime { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public interface IConnectionPoolService
    {
        Task<HttpClient> GetHttpClientAsync(string baseUrl, CancellationToken cancellationToken = default);
        void ReturnHttpClient(HttpClient httpClient);
        ConnectionPoolStats GetStats(string baseUrl);
        Task CleanupIdleConnectionsAsync(CancellationToken cancellationToken = default);
        void Dispose();
    }
}
