using EasyCaching.Core;
using Supabase;
using workoutAPI.Models;
using workoutAPI.Models.ApiKey;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Cache;
using workoutAPI.Models.Position;

namespace workoutAPI.Service;

public class StaticDataCacheService
{
    private readonly IEasyCachingProvider _cache;
    private readonly Client _supabase;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(12);
    private readonly ApiKeyService _apiKeyService;
    private readonly SemaphoreSlim _staticDataLoadLock = new SemaphoreSlim(1, 1);
    
    // Cache keys
    private const string PLANES_KEY = "planes";
    private const string BODY_REGIONS_KEY = "bodyRegions";
    private const string POSITIONS_KEY = "positions";
    private const string BODY_MOVEMENTS_KEY = "bodyMovements";
    private const string API_KEYS_KEY = "apiKeysForRecentDays";
    public StaticDataDTO AllData { get; set; } = new();
    
    public StaticDataCacheService(IEasyCachingProvider cache, Client supabase,  ApiKeyService apiKeyService)
    {
        _cache = cache;
        _supabase = supabase;
        _apiKeyService = apiKeyService;
    }

    
 public async Task<StaticDataDTO> GetStaticDataAsync()
{
    // Try to get all data from cache in parallel
    var planesCacheTask = _cache.GetAsync<List<PlaneDTO>>(PLANES_KEY);
    var bodyRegionsCacheTask = _cache.GetAsync<List<BodyRegionsDTO>>(BODY_REGIONS_KEY);
    var positionsCacheTask = _cache.GetAsync<List<PositionDTO>>(POSITIONS_KEY);
    var bodyMovementsCacheTask = _cache.GetAsync<List<BodyMovementDTO>>(BODY_MOVEMENTS_KEY);
    var apiKeysCacheTask = _cache.GetAsync<List<ApiKeyDTO>>(API_KEYS_KEY);

    await Task.WhenAll(planesCacheTask, bodyRegionsCacheTask, positionsCacheTask, bodyMovementsCacheTask, apiKeysCacheTask);

    var planesCache = await planesCacheTask;
    var bodyRegionsCache = await bodyRegionsCacheTask;
    var positionsCache = await positionsCacheTask;
    var bodyMovementsCache = await bodyMovementsCacheTask;
    var apiKeysCache = await apiKeysCacheTask;

    bool allDataInCache = planesCache.HasValue &&
                          bodyRegionsCache.HasValue &&
                          positionsCache.HasValue &&
                          bodyMovementsCache.HasValue &&
                          apiKeysCache.HasValue;

    if (allDataInCache)
    {
        return new StaticDataDTO
        {
            Planes = planesCache.Value,
            BodyRegions = bodyRegionsCache.Value,
            Positions = positionsCache.Value,
            BodyMovements = bodyMovementsCache.Value,
            ApiKeys = apiKeysCache.Value,
        };
    }

    // Cache miss - ensure only one loader runs concurrently
    await _staticDataLoadLock.WaitAsync();
    try
    {
        // Double-check cache after acquiring lock (another thread may have populated it)
        planesCache = await _cache.GetAsync<List<PlaneDTO>>(PLANES_KEY);
        bodyRegionsCache = await _cache.GetAsync<List<BodyRegionsDTO>>(BODY_REGIONS_KEY);
        positionsCache = await _cache.GetAsync<List<PositionDTO>>(POSITIONS_KEY);
        bodyMovementsCache = await _cache.GetAsync<List<BodyMovementDTO>>(BODY_MOVEMENTS_KEY);
        apiKeysCache = await _cache.GetAsync<List<ApiKeyDTO>>(API_KEYS_KEY);

        allDataInCache = planesCache.HasValue &&
                         bodyRegionsCache.HasValue &&
                         positionsCache.HasValue &&
                         bodyMovementsCache.HasValue &&
                         apiKeysCache.HasValue;

        if (allDataInCache)
        {
            return new StaticDataDTO
            {
                Planes = planesCache.Value,
                BodyRegions = bodyRegionsCache.Value,
                Positions = positionsCache.Value,
                BodyMovements = bodyMovementsCache.Value,
                ApiKeys = apiKeysCache.Value,
            };
        }

        // Still a miss, load from DB and cache
        Console.WriteLine("⚠️ Cache miss or expired - fetching static data from database (via LoadStaticDataAsync)");
        return await LoadStaticDataAsync();
    }
    finally
    {
        _staticDataLoadLock.Release();
    }
}
    
 public async Task<StaticDataDTO> LoadStaticDataAsync()
    {
        // Fetch from DB in parallel
        var planesTask = _supabase.From<PlaneDTO>().Get();
        var bodyRegionsTask = _supabase.From<BodyRegionsDTO>().Get();
        var positionsTask = _supabase.From<PositionDTO>().Get();
        var bodyMovementsTask = _supabase.From<BodyMovementDTO>().Get();
        var apiKeysTask = _apiKeyService.GetApiKeysForRecentDays();

        await Task.WhenAll(planesTask, bodyRegionsTask, positionsTask, bodyMovementsTask, apiKeysTask);

        var planes = planesTask.Result.Models.ToList();
        var bodyRegions = bodyRegionsTask.Result.Models.ToList();
        var positions = positionsTask.Result.Models.ToList();
        var bodyMovements = bodyMovementsTask.Result.Models.ToList();
        var apiKeys = await apiKeysTask;

        // Store each collection in cache with expiration
        await _cache.SetAsync(PLANES_KEY, planes, _cacheExpiration);
        await _cache.SetAsync(BODY_REGIONS_KEY, bodyRegions, _cacheExpiration);
        await _cache.SetAsync(POSITIONS_KEY, positions, _cacheExpiration);
        await _cache.SetAsync(BODY_MOVEMENTS_KEY, bodyMovements, _cacheExpiration);
        await _cache.SetAsync(API_KEYS_KEY, apiKeys, _cacheExpiration);

        Console.WriteLine("✅ Static data loaded from DB and cached successfully");

        return new StaticDataDTO
        {
            Planes = planes,
            BodyRegions = bodyRegions,
            Positions = positions,
            BodyMovements = bodyMovements,
            ApiKeys = apiKeys
        };
    }
    
  
 public async Task<(int? planeId, int? bodyRegionId, int? positionId, int? bodyMovementId)> 
        GetAllIdsByCodesAsync(string? planeCode, string? bodyRegionCode, string? positionCode, string? bodyMovementCode)
    {
        var staticData = await GetStaticDataAsync();
        return staticData.GetAllIds(planeCode, bodyRegionCode, positionCode, bodyMovementCode);
    }
   
    

    // Clear all static data from cache
    public async Task ClearCacheAsync()
    {
        await _cache.RemoveAsync(PLANES_KEY);
        await _cache.RemoveAsync(BODY_REGIONS_KEY);
        await _cache.RemoveAsync(POSITIONS_KEY);
        await _cache.RemoveAsync(BODY_MOVEMENTS_KEY);
        Console.WriteLine("🗑️ Static data cache cleared");
    }
    

    // Preload all static data on server start
    public async Task PreloadCacheAsync()
    {
        await GetStaticDataAsync();
        Console.WriteLine("✅ Static data cache preloaded");
    }
    

    // Check if cache exists and is not expired
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