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

        public async Task<decimal> EnsurePointsCachedAsync(string apiKey, Client supabase)
        {
            var cacheKey = CacheKey(apiKey);
            var cacheVal = await _cache.GetAsync<decimal>(cacheKey);
            if (cacheVal.HasValue) return cacheVal.Value;

            var sem = GetKeyLock(cacheKey);
            await sem.WaitAsync();
            try
            {
                cacheVal = await _cache.GetAsync<decimal>(cacheKey);  // ✅ decimal
                if (cacheVal.HasValue) return cacheVal.Value;

                var record = await supabase
                    .From<ApiKeyDTO>()
                    .Where(x => x.Key == apiKey)
                    .Get();

                decimal value = record.Model.ReqToday;
                
                var ttl = GetDefaultTtl();
                await _cache.SetAsync(cacheKey, value, ttl);

                return value;
            }
            finally
            {
                sem.Release();
            }
        }

        public async Task<decimal> AddPointsToCacheAsync(string apiKey, decimal delta)
        {
            var cacheKey = CacheKey(apiKey);
            var sem = GetKeyLock(cacheKey);
            await sem.WaitAsync();
            try
            {
                var cacheResult = await _cache.GetAsync<decimal>(cacheKey);  // ✅ decimal
                var cur = cacheResult.HasValue ? cacheResult.Value : 0m;     // ✅ 0m
                var next = cur + delta;
                await _cache.SetAsync(cacheKey, next, GetDefaultTtl());
                return next;
            }
            finally
            {
                sem.Release();
            }
        }

        public async Task<decimal> RemovePointsFromCacheAsync(string apiKey, decimal delta)
        {
            var cacheKey = CacheKey(apiKey);
            var sem = GetKeyLock(cacheKey);
            await sem.WaitAsync();
            try
            {
                var cacheResult = await _cache.GetAsync<decimal>(cacheKey);  // ✅ decimal
                var cur = cacheResult.HasValue ? cacheResult.Value : 0m;     // ✅ 0m
                var next = Math.Max(0m, cur - delta);                        // ✅ 0m
                await _cache.SetAsync(cacheKey, next, GetDefaultTtl());
                return next;
            }
            finally
            {
                sem.Release();
            }
        }

        public async Task WriteBackAsync(string apiKey, Client supabase)
        {
            var cacheKey = CacheKey(apiKey);
            var cacheResult = await _cache.GetAsync<decimal>(cacheKey); 
            var cur = cacheResult.HasValue ? cacheResult.Value : 0m;        
            
            await supabase
                .From<ApiKeyDTO>()
                .Where(x => x.Key == apiKey)
                .Set(x => x.ReqToday, cur)
                .Update(); 
        }

        public async Task SetQuotaAsync(string apiKey, decimal quota)  // ✅ decimal parameter
        {
            var cacheKey = CacheKey(apiKey);
            await _cache.SetAsync(cacheKey, quota, GetDefaultTtl());
        }

        private static string CacheKey(string apiKey) => $"quota:{apiKey}";

        private static TimeSpan GetDefaultTtl()
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.Date.AddDays(1);
            var ttl = tomorrow - now;
            if (ttl < TimeSpan.FromMinutes(1)) ttl = TimeSpan.FromMinutes(1);
            if (ttl > TimeSpan.FromHours(24)) ttl = TimeSpan.FromHours(24);
            return ttl;
        }
    }
}