using EasyCaching.Core;
using Supabase;
using workoutAPI.Models;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Cache;
using workoutAPI.Models.Position;

namespace workoutAPI.Service;

public class StaticDataCacheService
{
    private readonly IEasyCachingProvider _cache;
    private readonly Client _supabase;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(12);
    
    // Cache keys
    private const string PLANES_KEY = "planes";
    private const string BODY_REGIONS_KEY = "bodyRegions";
    private const string POSITIONS_KEY = "positions";
    private const string BODY_MOVEMENTS_KEY = "bodyMovements";
    
    public StaticDataCacheService(IEasyCachingProvider cache, Client supabase)
    {
        _cache = cache;
        _supabase = supabase;
    }

    /// <summary>
    /// Get all static data - checks cache first, fetches from DB if needed or expired
    /// </summary>
    public async Task<StaticDataDTO> GetStaticDataAsync()
    {
        // Try to get all data from cache in parallel
        var planesCacheTask = _cache.GetAsync<List<PlaneDTO>>(PLANES_KEY);
        var bodyRegionsCacheTask = _cache.GetAsync<List<BodyRegionsDTO>>(BODY_REGIONS_KEY);
        var positionsCacheTask = _cache.GetAsync<List<PositionDTO>>(POSITIONS_KEY);
        var bodyMovementsCacheTask = _cache.GetAsync<List<BodyMovementDTO>>(BODY_MOVEMENTS_KEY);
        
        await Task.WhenAll(planesCacheTask, bodyRegionsCacheTask, positionsCacheTask, bodyMovementsCacheTask);
        
        var planesCache = await planesCacheTask;
        var bodyRegionsCache = await bodyRegionsCacheTask;
        var positionsCache = await positionsCacheTask;
        var bodyMovementsCache = await bodyMovementsCacheTask;
        
        // Check if ALL data exists in cache AND hasn't expired
        // EasyCaching automatically returns HasValue = false if data is expired
        bool allDataInCache = planesCache.HasValue && 
                              bodyRegionsCache.HasValue && 
                              positionsCache.HasValue && 
                              bodyMovementsCache.HasValue;
        
        if (allDataInCache)
        {
            // All data in cache and not expired, return it
            return new StaticDataDTO
            {
                Planes = planesCache.Value,
                BodyRegions = bodyRegionsCache.Value,
                Positions = positionsCache.Value,
                BodyMovements = bodyMovementsCache.Value
            };
        }
        
        // Cache miss OR expired - fetch from database in parallel
        Console.WriteLine("⚠️ Cache miss or expired - fetching static data from database");
        
        var planesTask = _supabase.From<PlaneDTO>().Get();
        var bodyRegionsTask = _supabase.From<BodyRegionsDTO>().Get();
        var positionsTask = _supabase.From<PositionDTO>().Get();
        var bodyMovementsTask = _supabase.From<BodyMovementDTO>().Get();
        
        await Task.WhenAll(planesTask, bodyRegionsTask, positionsTask, bodyMovementsTask);
        
        var planes = planesTask.Result.Models.ToList();
        var bodyRegions = bodyRegionsTask.Result.Models.ToList();
        var positions = positionsTask.Result.Models.ToList();
        var bodyMovements = bodyMovementsTask.Result.Models.ToList();
        
        // Store each collection in cache with expiration
        // EasyCaching will automatically expire these after _cacheExpiration
        await _cache.SetAsync(PLANES_KEY, planes, _cacheExpiration);
        await _cache.SetAsync(BODY_REGIONS_KEY, bodyRegions, _cacheExpiration);
        await _cache.SetAsync(POSITIONS_KEY, positions, _cacheExpiration);
        await _cache.SetAsync(BODY_MOVEMENTS_KEY, bodyMovements, _cacheExpiration);
        
        Console.WriteLine("✅ Static data cached successfully");
        
        return new StaticDataDTO
        {
            Planes = planes,
            BodyRegions = bodyRegions,
            Positions = positions,
            BodyMovements = bodyMovements
        };
    }
    
    /// <summary>
    /// Get all IDs by codes - uses cached static data
    /// </summary>
    public async Task<(int? planeId, int? bodyRegionId, int? positionId, int? bodyMovementId)> 
        GetAllIdsByCodesAsync(string? planeCode, string? bodyRegionCode, string? positionCode, string? bodyMovementCode)
    {
        var staticData = await GetStaticDataAsync();
        return staticData.GetAllIds(planeCode, bodyRegionCode, positionCode, bodyMovementCode);
    }
   
    /// <summary>
    /// Clear all static data from cache
    /// </summary>
    public async Task ClearCacheAsync()
    {
        await _cache.RemoveAsync(PLANES_KEY);
        await _cache.RemoveAsync(BODY_REGIONS_KEY);
        await _cache.RemoveAsync(POSITIONS_KEY);
        await _cache.RemoveAsync(BODY_MOVEMENTS_KEY);
        Console.WriteLine("🗑️ Static data cache cleared");
    }
    
    /// <summary>
    /// Preload all static data on server start
    /// </summary>
    public async Task PreloadCacheAsync()
    {
        await GetStaticDataAsync();
        Console.WriteLine("✅ Static data cache preloaded");
    }
    
    /// <summary>
    /// Check if cache exists and is not expired
    /// </summary>
    public async Task<bool> IsCacheValidAsync()
    {
        var planesCacheTask = _cache.ExistsAsync(PLANES_KEY);
        var bodyRegionsCacheTask = _cache.ExistsAsync(BODY_REGIONS_KEY);
        var positionsCacheTask = _cache.ExistsAsync(POSITIONS_KEY);
        var bodyMovementsCacheTask = _cache.ExistsAsync(BODY_MOVEMENTS_KEY);
        
        await Task.WhenAll(planesCacheTask, bodyRegionsCacheTask, positionsCacheTask, bodyMovementsCacheTask);
        
        return await planesCacheTask && 
               await bodyRegionsCacheTask && 
               await positionsCacheTask && 
               await bodyMovementsCacheTask;
    }
}