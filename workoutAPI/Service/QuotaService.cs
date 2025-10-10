
using System.Collections.Concurrent;
using EasyCaching.Core;
using Supabase;
using workoutAPI.Models.ApiKey;

namespace workoutAPI.Service
{
    public class QuotaService
    {
        private readonly IEasyCachingProvider _cache;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> KeyLocks = new();

        public QuotaService(IEasyCachingProviderFactory cacheFactory)
        {
            _cache = cacheFactory.GetCachingProvider("default");
        }

        private SemaphoreSlim GetKeyLock(string key) =>
            KeyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        // Lazy-load + safe init of cache
        public async Task<int> EnsurePointsCachedAsync(string apiKey, Client supabase)
        {
            var cacheKey = CacheKey(apiKey);
            var cacheVal = await _cache.GetAsync<int>(cacheKey);
            if (cacheVal.HasValue) return cacheVal.Value;

            var sem = GetKeyLock(cacheKey);
            await sem.WaitAsync();
            try
            {
                cacheVal = await _cache.GetAsync<int>(cacheKey);
                if (cacheVal.HasValue) return cacheVal.Value;

                // hämta från DB - SingleAsync är tydligt
                var record = await supabase
                    .From<ApiKeyDTO>()
                    .Where(x => x.Key == apiKey)
                    .Get();

                var value = record.Model.ReqToday;

                // Sätt TTL till tid kvar till nästa reset som default
                var ttl = GetDefaultTtl();
                await _cache.SetAsync(cacheKey, value, ttl);

                return value;
            }
            finally
            {
                sem.Release();
            }
        }


        public async Task<int> AddPointsToCacheAsync(string apiKey, int delta)
        {
            var cacheKey = CacheKey(apiKey);
            var sem = GetKeyLock(cacheKey);
            await sem.WaitAsync();
            try
            {
                var cur = (await _cache.GetAsync<int>(cacheKey)).HasValue ? (await _cache.GetAsync<int>(cacheKey)).Value : 0;
                var next = cur + delta;
                await _cache.SetAsync(cacheKey, next, GetDefaultTtl());
                return next;
            }
            finally
            {
                sem.Release();
            }
        }

        // Rollback: subtract delta
        public async Task<int> RemovePointsFromCacheAsync(string apiKey, int delta)
        {
            var cacheKey = CacheKey(apiKey);
            var sem = GetKeyLock(cacheKey);
            await sem.WaitAsync();
            try
            {
                var cur = (await _cache.GetAsync<int>(cacheKey)).HasValue ? (await _cache.GetAsync<int>(cacheKey)).Value : 0;
                var next = Math.Max(0, cur - delta);
                await _cache.SetAsync(cacheKey, next, GetDefaultTtl());
                return next;
            }
            finally
            {
                sem.Release();
            }
        }

        // Write-back cache value to DB (persist)
        public async Task WriteBackAsync(string apiKey, Client supabase)
        {
            var cacheKey = CacheKey(apiKey);
            var cur = (await _cache.GetAsync<int>(cacheKey)).HasValue ? (await _cache.GetAsync<int>(cacheKey)).Value : 0;

            
            await supabase
                .From<ApiKeyDTO>()
                .Where(x => x.Key == apiKey)
                .Set(x => x.ReqToday, cur)
                .Get();
        }

        // Helper: sätt quota i cache (extern användning)
        public async Task SetQuotaAsync(string apiKey, int quota)
        {
            var cacheKey = CacheKey(apiKey);
            await _cache.SetAsync(cacheKey, quota, GetDefaultTtl());
        }

        // --- Helpers ---
        private static string CacheKey(string apiKey) => $"quota:{apiKey}";

        // Default TTL: tid kvar till nästa UTC-midnatt (anpassa om reset är annorlunda)
        private static TimeSpan GetDefaultTtl()
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.Date.AddDays(1);
            var ttl = tomorrow - now;
            // Minimum 1 minute, maximum 24h
            if (ttl < TimeSpan.FromMinutes(1)) ttl = TimeSpan.FromMinutes(1);
            if (ttl > TimeSpan.FromHours(24)) ttl = TimeSpan.FromHours(24);
            return ttl;
        }
    }
}
