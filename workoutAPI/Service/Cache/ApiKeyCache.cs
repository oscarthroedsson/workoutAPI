using EasyCaching.Core;
using Supabase;
using workoutAPI.Models.ApiKey;

namespace workoutAPI.Service.Cache;


public class ApiKeyCache
{
    private readonly IEasyCachingProvider _cache;
    private readonly Client _supabase;
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(15);
    

    
    public ApiKeyCache(IEasyCachingProvider cache, Client supabase)
    {
        _cache = cache;
        _supabase = supabase;
    }
    
    public async Task<ApiKeyDTO?> GetUserApiKeyAsync(string apiKey)
    {
        var cacheKey = $"{apiKey}";
        
        // Try to get from cache first
        var cached = await _cache.GetAsync<ApiKeyDTO>(cacheKey);
        if (cached.HasValue) return cached.Value;
        
        
        // Cache miss - fetch from database with user join
        var apiKeyResponse = await _supabase
            .From<ApiKeyDTO>()
            .Select("*, Users!ApiKeys_user_id_fkey(*)")  // ← Exakt namnet på din FK constraint
            .Where(x => x.Key == apiKey)
            .Single();

        
        if (apiKeyResponse == null) return null;
        
        
        await _cache.SetAsync(cacheKey, apiKeyResponse, _expiration);
        
        return apiKeyResponse;
    }
    
   // Remove a certain key from cache
    public async Task RemoveKeyCache(string apiKey)
    {
        var cacheKey = $"{apiKey}";
        await _cache.RemoveAsync(cacheKey);
    }
    
    public async Task InvalidateByIdAsync(string apiKeyId)
    {
        var apiKeyResponse = await _supabase
            .From<ApiKeyDTO>()
            .Select("key")
            .Where(x => x.Id == apiKeyId)
            .Single();
        
        if (apiKeyResponse != null)  await RemoveKeyCache(apiKeyResponse.Key);
    }
    
    // Clear all the keys 
    public async Task ClearAllAsync()
    {
        await _cache.FlushAsync();
        Console.WriteLine("🗑️ All API key caches cleared");
    }
}