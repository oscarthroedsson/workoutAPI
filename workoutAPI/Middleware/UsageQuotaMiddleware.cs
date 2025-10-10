using System.Text.Json;
using Supabase;
using workoutAPI.Headers;
using workoutAPI.Models;
using workoutAPI.Models.ApiKey;
using workoutAPI.Models.User;
using workoutAPI.Service;

namespace workoutAPI.Middlewear;

public class UsageQuotaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly QuotaService _quotaService;
    private readonly Client _supabase;
    private readonly HeaderManager _headerManager;

    public UsageQuotaMiddleware(
        RequestDelegate next,
        QuotaService quotaService,
        Client supabase
        )
    {
        _next = next;
        _quotaService = quotaService;
        _supabase = supabase;
    }

    public async Task InvokeAsync(HttpContext context, BillingService billingService)
    {
        var headerManager = context.RequestServices.GetRequiredService<HeaderManager>();
        var apiKeyDto = context.Items["ApiKey"] as ApiKeyDTO;
        var userDTO = context.Items["User"] as UserDTO;
        var requestPoints = (int)(context.Items["RequestPoints"] ?? 0);

        if (apiKeyDto == null || userDTO == null)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(JSONResponse.Error("Missing authentication data")));
            return;
        }

        var tier = userDTO.Tier.ToLowerInvariant();
        var apiKeyString = apiKeyDto.Key;

        // Reset usage if period changed
        bool needsReset = ShouldResetUsage(apiKeyDto);
        if (needsReset)
        {
            apiKeyDto.LastResetDate = DateTime.UtcNow;
            apiKeyDto.ReqToday = 0;
            await _quotaService.SetQuotaAsync(apiKeyString, 0);
        }

        // Ensure points in cache
        await _quotaService.EnsurePointsCachedAsync(apiKeyString, _supabase);
        // Reserve points
        var newTotal = await _quotaService.AddPointsToCacheAsync(apiKeyString, requestPoints);
        var limit = TierService.GetTierLimit(tier);
        
        // Over quota
        if (newTotal > limit)
        {
            await _quotaService.RemovePointsFromCacheAsync(apiKeyString, requestPoints);

            context.Response.StatusCode = 402;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(JSONResponse.Error("You need to update your plan or wait for reset", "PAYMENT_REQUIRED")));
            return;
        }

        // Set WithinLimit header
        headerManager.SetWithinLimit(newTotal <= limit);
        // Billing & headers
        var currentPeriod = DateTime.UtcNow.ToString("yyyy-MM");
        var billingRecord = await billingService.GetOrCreateBillingRecord(userDTO, apiKeyDto, currentPeriod);

        if (TierService.ShouldBeBilled(tier, billingRecord.TotalPointsUsed, requestPoints))
        {
            var overagePoints = TierService.GetOveragePoints(billingRecord.TotalPointsUsed + requestPoints, billingRecord.IncludedPoints);
            var overageCost = TierService.CalculateOverageCost(tier, overagePoints);
            
            headerManager.AddHeaders(
                (HeaderKey.QuotaExceeded, "true"),
                (HeaderKey.OveragePoints, overagePoints.ToString()),
                (HeaderKey.OverageCost, $"${overageCost:F2}")
            );
        }
        else
        {
            headerManager.AddHeaders(
                (HeaderKey.QuotaExceeded, "false"),
                (HeaderKey.OverageCost, "0.00")
            );
        }

        // Points requested header
        headerManager.AddHeader(HeaderKey.PointsRequested, requestPoints.ToString());

        try
        {
            await _next(context);
            // 🍒 Fire-and-forget: Run both operations in background without blocking response
            if (context.Response.StatusCode < 400)
            { 
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.WhenAll(
                            _supabase.From<ApiKeyDTO>()
                                .Where(x => x.Key == apiKeyDto.Key)
                                .Set(x => x.ReqToday, newTotal)
                                .Update(),
                
                            _supabase.From<ApiUsageDTO>().Insert(
                                new ApiUsageDTO
                                {
                                    ApiKeyId = apiKeyDto.Id,
                                    Endpoint = context.Request.Path,
                                    PointCost = requestPoints,
                                }
                            )
                        );
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't fail the request since it already succeeded
                        Console.WriteLine($"❌ Failed to persist usage data: {ex.Message}");
                    }
                });
            }
        }
        catch (Exception)
        {
            await _quotaService.RemovePointsFromCacheAsync(apiKeyString, requestPoints);
            throw;
        }
        

        // Persist usage
        _ = Task.Run(async () =>
        {
            try
            {
                await _quotaService.WriteBackAsync(apiKeyString, _supabase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to writeback: {ex.Message}");
            }
        });
        apiKeyDto.ReqToday = newTotal;
    }

    private bool ShouldResetUsage(ApiKeyDTO apiKeyDto)
    {
        var now = DateTime.UtcNow.Date;
        var lastReset = apiKeyDto.LastResetDate.Date;
        return now > lastReset;
    }

    
    private async Task ResetUsage(ApiKeyDTO apiKeyDto)
    {
        Console.WriteLine("🤖 ResetUsage was run");
        
        apiKeyDto.LastResetDate = DateTime.UtcNow;
        apiKeyDto.ReqToday = 0;

        await _supabase
            .From<ApiKeyDTO>()
            .Where(x => x.Key == apiKeyDto.Key)
            .Set(x => x.ReqToday, 0)
            .Set(x => x.LastResetDate, apiKeyDto.LastResetDate)
            .Update();
    }
}
