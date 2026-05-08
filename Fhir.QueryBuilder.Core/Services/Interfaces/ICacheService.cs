namespace Fhir.QueryBuilder.Services.Interfaces
{
    /// <summary>
    /// 快取服務介面
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 取得快取值
        /// </summary>
        /// <typeparam name="T">值類型</typeparam>
        /// <param name="key">快取鍵</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>快取值</returns>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 設定快取值
        /// </summary>
        /// <typeparam name="T">值類型</typeparam>
        /// <param name="key">快取鍵</param>
        /// <param name="value">值</param>
        /// <param name="expiration">過期時間</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 移除快取值
        /// </summary>
        /// <param name="key">快取鍵</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// 清除所有快取
        /// </summary>
        /// <param name="cancellationToken">取消權杖</param>
        Task ClearAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 檢查快取是否存在
        /// </summary>
        /// <param name="key">快取鍵</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根據模式移除快取
        /// </summary>
        /// <param name="pattern">模式</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得快取統計資訊
        /// </summary>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>統計資訊</returns>
        Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 快取統計資訊
    /// </summary>
    public class CacheStatistics
    {
        /// <summary>
        /// 快取項目數量
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 命中次數
        /// </summary>
        public long HitCount { get; set; }

        /// <summary>
        /// 未命中次數
        /// </summary>
        public long MissCount { get; set; }

        /// <summary>
        /// 命中率
        /// </summary>
        public double HitRate => HitCount + MissCount > 0 ? (double)HitCount / (HitCount + MissCount) : 0;

        /// <summary>
        /// 記憶體使用量（位元組）
        /// </summary>
        public long MemoryUsage { get; set; }
    }
}
