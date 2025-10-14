using EasyCaching.Core;
using Supabase;
using workoutAPI.Service;
using workoutAPI.Service.Cache;

public class CacheManager
{
    private readonly IEasyCachingProvider _cacheProvider;
    private readonly Client _supabase;
    private readonly ApiKeyService _apiKeyService;
    private ApiKeyCache? _apiKeys;
    private StaticDataCacheService? _staticData;
    
    public CacheManager(IEasyCachingProvider cacheProvider, Client supabase,ApiKeyService apiKeyService,  StaticDataCacheService staticDataCacheService)  
    {
        _cacheProvider = cacheProvider;
        _supabase = supabase;
        _apiKeyService = apiKeyService; // ← Spara här
        _staticData = staticDataCacheService;
    }
    
    // Provide access to different caching instances
    public ApiKeyCache ApiKeys => _apiKeys ??= new ApiKeyCache(_cacheProvider, _supabase);
    public StaticDataCacheService StaticData => _staticData ??= new StaticDataCacheService(_cacheProvider, _supabase,_apiKeyService);
}


