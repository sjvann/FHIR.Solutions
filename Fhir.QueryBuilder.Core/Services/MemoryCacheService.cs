using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Fhir.QueryBuilder.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
        private readonly ILogger<MemoryCacheService> _logger;
        private long _hitCount = 0;
        private long _missCount = 0;

        public MemoryCacheService(ILogger<MemoryCacheService> logger)
        {
            _logger = logger;
        }

        private class CacheItem
        {
            public object Value { get; set; } = null!;
            public DateTime ExpirationTime { get; set; }
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                if (_cache.TryGetValue(key, out var cacheItem))
                {
                    if (DateTime.UtcNow <= cacheItem.ExpirationTime)
                    {
                        Interlocked.Increment(ref _hitCount);
                        _logger.LogDebug("Cache HIT for key: {Key}", key);
                        return Task.FromResult((T?)cacheItem.Value);
                    }
                    else
                    {
                        _cache.TryRemove(key, out _);
                        _logger.LogDebug("Cache EXPIRED for key: {Key}", key);
                    }
                }

                Interlocked.Increment(ref _missCount);
                _logger.LogDebug("Cache MISS for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var expirationTime = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromMinutes(30));
                var cacheItem = new CacheItem
                {
                    Value = value,
                    ExpirationTime = expirationTime
                };

                _cache.AddOrUpdate(key, cacheItem, (k, v) => cacheItem);
                _logger.LogDebug("Cache SET for key: {Key} with expiration: {Expiration}", key, expiration);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _cache.TryRemove(key, out _);
                _logger.LogDebug("Cache REMOVE for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _cache.Clear();
                _logger.LogDebug("Cache CLEARED");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache");
                return Task.CompletedTask;
            }
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = _cache.ContainsKey(key);
                if (exists && _cache.TryGetValue(key, out var cacheItem))
                {
                    if (DateTime.UtcNow > cacheItem.ExpirationTime)
                    {
                        _cache.TryRemove(key, out _);
                        exists = false;
                    }
                }

                _logger.LogDebug("Cache EXISTS check for key: {Key} - {Exists}", key, exists);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return Task.FromResult(false);
            }
        }

        public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                // 將萬用字元模式轉換為正規表達式
                var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
                var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

                var keysToRemove = _cache.Keys
                    .Where(key => regex.IsMatch(key))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _cache.TryRemove(key, out _);
                }

                _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}",
                    keysToRemove.Count, pattern);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache entries by pattern: {Pattern}", pattern);
                return Task.CompletedTask;
            }
        }

        public Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // 清理過期項目
                var expiredKeys = _cache
                    .Where(kvp => DateTime.UtcNow > kvp.Value.ExpirationTime)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in expiredKeys)
                {
                    _cache.TryRemove(key, out _);
                }

                var statistics = new CacheStatistics
                {
                    ItemCount = _cache.Count,
                    HitCount = _hitCount,
                    MissCount = _missCount,
                    MemoryUsage = EstimateMemoryUsage()
                };

                return Task.FromResult(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache statistics");
                return Task.FromResult(new CacheStatistics());
            }
        }

        private long EstimateMemoryUsage()
        {
            // 簡化的記憶體使用量估算
            // 實際實作可能需要更精確的計算
            return _cache.Count * 1024; // 假設每個項目平均 1KB
        }
    }
}
