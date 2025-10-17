using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using workoutAPI.Headers;
using workoutAPI.Models;
using workoutAPI.Service;
using workoutAPI.Service.Cache;

namespace workoutAPI.Middlewear
{
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimiterCache _rateLimiterCache;
        private readonly ApiKeyService _apiKeyService;

       
        
        public RateLimiterMiddleware(RequestDelegate next, RateLimiterCache rateLimiterCache, ApiKeyService apiKeyService)
        {
            _next = next;
            _rateLimiterCache = rateLimiterCache;
            _apiKeyService = apiKeyService;

            Console.WriteLine("[RateLimiterCache] Initialized");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var apiKey = context.Request.Headers["X-Api-Key"].ToString();
    
            if (string.IsNullOrEmpty(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(JSONResponse.Error(
                    "Missing API Key",
                    code: "MISSING_API_KEY"
                ));
                return;
            }

            // Get user info → Cache → DB
            var userInfo = await _apiKeyService.GetUserInfoByApiKeyAsync(apiKey);
    
            if (userInfo == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(JSONResponse.Error(
                    "Invalid or inactive API key",
                    code: "INVALID_API_KEY"
                ));
                return;
            }

            var tier = userInfo.Tier;
            // Validate tier
            if (!TierService.IsValidTier(tier))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(JSONResponse.Error(
                    "Invalid tier configuration",
                    code: "INVALID_TIER"
                ));
                return;
            }

            // limit req/sex and conc
            var rateLimit = TierService.GetTierRateLimit(tier);

            // Update rate limiter
            var rateEntry = await _rateLimiterCache.Upsert(apiKey, tier);

            Console.WriteLine($"Amount: {rateEntry.Amount}");
            Console.WriteLine($"Expiry: {rateEntry.Expiry}");
            Console.WriteLine($"Limit: {rateLimit}");
            
            if (rateEntry.Amount > rateLimit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(JSONResponse.Error(
                    $"Rate limit exceeded. Max {rateLimit} requests per {rateLimit} seconds.",
                    code: "RATE_LIMIT_EXCEEDED"
                ));
                return;
            }
            
            
            
            // Continue
            await _next(context);
        }
    }
}