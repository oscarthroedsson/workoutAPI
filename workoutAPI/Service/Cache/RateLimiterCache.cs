using System.Collections.Concurrent;
using EasyCaching.Core;
using Supabase;
using workoutAPI.Models.RateLimiter;

namespace workoutAPI.Service.Cache;

public class RateLimiterCache
{
    private readonly IEasyCachingProvider _cache;
    private readonly Dictionary<string, int> _tierRateLimits;
    private readonly ConcurrentDictionary<string, RateLimitEntry> _concurrentCounters;
    private readonly string PREFIX_KEY = "rate-limiter";

    public RateLimiterCache(IEasyCachingProvider cache)
    {
        _cache = cache;
        _tierRateLimits = TierService.GetAllTierRateLimits();
        _concurrentCounters = new ConcurrentDictionary<string, RateLimitEntry>();
    }
    
    
    public async Task<RateLimitEntry> Upsert(string apiKey, string tier)
    {
        if (!TierService.IsValidTier(tier.ToUpperInvariant())) throw new ArgumentException($"Unknown tier: {tier}");
        
        var tierRateLimit = TierService.GetTierRateLimit(tier);
        
        
        var key = $"{PREFIX_KEY}_{apiKey}";
        var entry = _concurrentCounters.AddOrUpdate(key,
            addValueFactory: k => new RateLimitEntry
            {
                Amount = 1,
                Expiry = DateTime.UtcNow.AddSeconds(1)
            },
            updateValueFactory: (k, existing) =>
            {
                // Om TTL har gått ut så nollställ count
                if (existing.Expiry < DateTime.UtcNow)
                {
                    existing.Amount = 1;
                    existing.Expiry = DateTime.UtcNow.AddSeconds(1);
                }
                else
                {
                    existing.Amount++;
                }
                
                
                return existing;
            });
        
        
        await _cache.SetAsync(key, entry, entry.Expiry - DateTime.UtcNow);
        Console.WriteLine($"[RateLimiterCache] Updated entry for {apiKey}: Amount={entry.Amount}, Expiry={entry.Expiry}");

        return entry;
    }
}