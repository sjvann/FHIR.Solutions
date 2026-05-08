using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Fhir.QueryBuilder.Services
{
    public class ConnectionPoolService : IConnectionPoolService, IDisposable
    {
        private readonly ILogger<ConnectionPoolService> _logger;
        private readonly QueryBuilderAppSettings _options;
        private readonly ConcurrentDictionary<string, ConnectionPool> _pools = new();
        private readonly System.Threading.Timer _cleanupTimer;
        private bool _disposed;

        public ConnectionPoolService(
            ILogger<ConnectionPoolService> logger,
            IOptions<QueryBuilderAppSettings> options)
        {
            _logger = logger;
            _options = options.Value;
            
            // Setup cleanup timer to run every 5 minutes
            _cleanupTimer = new System.Threading.Timer(async _ => await CleanupIdleConnectionsAsync(),
                null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public async Task<HttpClient> GetHttpClientAsync(string baseUrl, CancellationToken cancellationToken = default)
        {
            var pool = _pools.GetOrAdd(baseUrl, url => new ConnectionPool(url, _options, _logger));
            return await pool.GetClientAsync(cancellationToken);
        }

        public void ReturnHttpClient(HttpClient httpClient)
        {
            // In this implementation, we don't actually return clients to the pool
            // as HttpClient is designed to be long-lived and reused
            // The pool manages the lifecycle internally
        }

        public ConnectionPoolStats GetStats(string baseUrl)
        {
            if (_pools.TryGetValue(baseUrl, out var pool))
            {
                return pool.GetStats();
            }

            return new ConnectionPoolStats();
        }

        public async Task CleanupIdleConnectionsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Starting connection pool cleanup");

            var cleanupTasks = _pools.Values.Select(pool => pool.CleanupAsync(cancellationToken));
            await Task.WhenAll(cleanupTasks);

            _logger.LogDebug("Connection pool cleanup completed");
        }

        public void Dispose()
        {
            if (_disposed) return;

            _cleanupTimer?.Dispose();
            
            foreach (var pool in _pools.Values)
            {
                pool.Dispose();
            }
            
            _pools.Clear();
            _disposed = true;
        }

        private class ConnectionPool : IDisposable
        {
            private readonly string _baseUrl;
            private readonly QueryBuilderAppSettings _options;
            private readonly ILogger _logger;
            private readonly SemaphoreSlim _semaphore;
            private readonly ConcurrentQueue<PooledHttpClient> _availableClients = new();
            private readonly ConcurrentDictionary<HttpClient, PooledHttpClient> _allClients = new();
            private readonly object _statsLock = new();
            private int _totalConnections;
            private int _activeConnections;
            private DateTime _lastActivity = DateTime.UtcNow;

            public ConnectionPool(string baseUrl, QueryBuilderAppSettings options, ILogger logger)
            {
                _baseUrl = baseUrl;
                _options = options;
                _logger = logger;
                _semaphore = new SemaphoreSlim(options.Performance.MaxConcurrentRequests, options.Performance.MaxConcurrentRequests);
            }

            public async Task<HttpClient> GetClientAsync(CancellationToken cancellationToken = default)
            {
                await _semaphore.WaitAsync(cancellationToken);

                try
                {
                    // Try to get an available client
                    if (_availableClients.TryDequeue(out var pooledClient) && !pooledClient.IsExpired)
                    {
                        pooledClient.MarkAsActive();
                        Interlocked.Increment(ref _activeConnections);
                        _lastActivity = DateTime.UtcNow;
                        return pooledClient.HttpClient;
                    }

                    // Create new client if under limit
                    if (_totalConnections < _options.Performance.MaxConcurrentRequests)
                    {
                        var newClient = CreateHttpClient();
                        var newPooledClient = new PooledHttpClient(newClient, _options.Performance.ConnectionPoolTimeoutSeconds);
                        
                        _allClients.TryAdd(newClient, newPooledClient);
                        newPooledClient.MarkAsActive();
                        
                        Interlocked.Increment(ref _totalConnections);
                        Interlocked.Increment(ref _activeConnections);
                        _lastActivity = DateTime.UtcNow;
                        
                        _logger.LogDebug("Created new HTTP client for {BaseUrl}. Total: {Total}", _baseUrl, _totalConnections);
                        return newClient;
                    }

                    // Wait for an available client
                    var timeout = TimeSpan.FromSeconds(_options.Performance.ConnectionPoolTimeoutSeconds);
                    using var timeoutCts = new CancellationTokenSource(timeout);
                    using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                    while (!combinedCts.Token.IsCancellationRequested)
                    {
                        if (_availableClients.TryDequeue(out pooledClient) && !pooledClient.IsExpired)
                        {
                            pooledClient.MarkAsActive();
                            Interlocked.Increment(ref _activeConnections);
                            _lastActivity = DateTime.UtcNow;
                            return pooledClient.HttpClient;
                        }

                        await Task.Delay(100, combinedCts.Token);
                    }

                    throw new TimeoutException($"Timeout waiting for available HTTP client for {_baseUrl}");
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            public ConnectionPoolStats GetStats()
            {
                lock (_statsLock)
                {
                    return new ConnectionPoolStats
                    {
                        TotalConnections = _totalConnections,
                        ActiveConnections = _activeConnections,
                        IdleConnections = _totalConnections - _activeConnections,
                        PendingRequests = _options.Performance.MaxConcurrentRequests - _semaphore.CurrentCount,
                        LastActivity = _lastActivity
                    };
                }
            }

            public async Task CleanupAsync(CancellationToken cancellationToken = default)
            {
                var expiredClients = new List<PooledHttpClient>();

                // Find expired clients
                foreach (var kvp in _allClients)
                {
                    if (kvp.Value.IsExpired)
                    {
                        expiredClients.Add(kvp.Value);
                    }
                }

                // Remove expired clients
                foreach (var expiredClient in expiredClients)
                {
                    if (_allClients.TryRemove(expiredClient.HttpClient, out _))
                    {
                        expiredClient.Dispose();
                        Interlocked.Decrement(ref _totalConnections);
                        _logger.LogDebug("Removed expired HTTP client for {BaseUrl}", _baseUrl);
                    }
                }

                // Clean up the available queue
                var validClients = new List<PooledHttpClient>();
                while (_availableClients.TryDequeue(out var client))
                {
                    if (!client.IsExpired)
                    {
                        validClients.Add(client);
                    }
                }

                foreach (var client in validClients)
                {
                    _availableClients.Enqueue(client);
                }
            }

            private HttpClient CreateHttpClient()
            {
                var handler = new HttpClientHandler();
                
                if (_options.Performance.EnableCompression)
                {
                    handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
                }

                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(_baseUrl),
                    Timeout = TimeSpan.FromSeconds(_options.RequestTimeoutSeconds)
                };

                // Add custom headers
                foreach (var header in _options.CustomHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                client.DefaultRequestHeaders.Add("User-Agent", "Fhir.QueryBuilder/1.0");
                
                return client;
            }

            public void Dispose()
            {
                _semaphore?.Dispose();
                
                foreach (var client in _allClients.Values)
                {
                    client.Dispose();
                }
                
                _allClients.Clear();
            }
        }

        private class PooledHttpClient : IDisposable
        {
            public HttpClient HttpClient { get; }
            public DateTime CreatedAt { get; }
            public DateTime LastUsed { get; private set; }
            public bool IsActive { get; private set; }
            private readonly int _timeoutSeconds;

            public PooledHttpClient(HttpClient httpClient, int timeoutSeconds)
            {
                HttpClient = httpClient;
                CreatedAt = DateTime.UtcNow;
                LastUsed = DateTime.UtcNow;
                _timeoutSeconds = timeoutSeconds;
            }

            public bool IsExpired => DateTime.UtcNow - LastUsed > TimeSpan.FromSeconds(_timeoutSeconds);

            public void MarkAsActive()
            {
                IsActive = true;
                LastUsed = DateTime.UtcNow;
            }

            public void MarkAsIdle()
            {
                IsActive = false;
                LastUsed = DateTime.UtcNow;
            }

            public void Dispose()
            {
                HttpClient?.Dispose();
            }
        }
    }
}
