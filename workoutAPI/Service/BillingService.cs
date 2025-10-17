using EasyCaching.Core;
using Supabase;
using workoutAPI.Models.Billing;
using workoutAPI.Models.User;
using workoutAPI.Models.ApiKey;

namespace workoutAPI.Service;

public class BillingService
{
    private readonly IEasyCachingProvider _cache;
    private readonly Client _supabase;

    
    public BillingService(IEasyCachingProviderFactory cacheFactory, Client supabase)
    {
        // FORMAT {apiKey:Period} yyyy-MM
        _cache = cacheFactory.GetCachingProvider("BillingRecord");
        _supabase = supabase;
    }

    public async Task<BillingRecordDTO> GetOrCreateBillingRecord(
        UserDTO user, 
        ApiKeyDTO apiKey, 
        DateTime period)
    {
        // Try to get from cache
        var cacheValue = await _cache.GetAsync<BillingRecordDTO>($@"{apiKey.Key}:{period.ToString("yyyy-MM")}");
        if (cacheValue.HasValue) return cacheValue.Value;
       
        BillingRecordDTO response = null;
        try
        {
            // Try to get from DB
            response = await _supabase
                .From<BillingRecordDTO>()
                .Where(x => x.UserId == user.Id)
                .Where(x => x.BillingPeriod == period.ToString())
                .Single();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to fetch billing record: {ex.Message}");
        }


        if (response != null)
        {
            // Update cache if previous is success
            await _cache.SetAsync(apiKey.Key, response, TimeSpan.FromDays(1));
            return response;
        }
            // If none above is success ↓
            // Create new billing record for this month
            var tierLimit = TierService.GetTierLimit(user.Tier);
            var baseCost = TierService.GetBaseCost(user.Tier);

            var newRecord = new BillingRecordDTO
            {
                UserId = user.Id,
                ApiKeyId = apiKey.Id,
                BillingPeriod = period.ToString("yyyy-MM"),
                TotalRequests = 0,
                TotalPointsUsed = 0,
                IncludedPoints = tierLimit,
                OveragePoints = 0,
                BaseSubscriptionCost = baseCost,
                OverageCost = 0m,
                TotalCost = baseCost,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            };
            
            var cacheTask = _cache.SetAsync($@"{apiKey.Key}:{period.ToString("yyyy-MM")}", newRecord, TimeSpan.FromDays(1));
            var dbTask = _supabase.From<BillingRecordDTO>().Insert(newRecord);
            await Task.WhenAll(cacheTask, dbTask);

            return newRecord;
    }


 
    
    
    // Returns updated BillingRecord
    public async Task<BillingRecordDTO> UpsertBilling(
        UserDTO userDTO, 
        ApiKeyDTO apiKeyDTO,
        DateTime period, 
        decimal requestPoints, 
        string tier
        )
    {
        var billingRecord = await GetOrCreateBillingRecord(userDTO, apiKeyDTO, period);
        // Update totals
        billingRecord.TotalRequests += 1;
        billingRecord.TotalPointsUsed += requestPoints;

        // Calculate overage (no risk of double counting)
        billingRecord.OveragePoints = TierService.GetTotalOveragePoints(
            billingRecord.TotalPointsUsed, 
            billingRecord.IncludedPoints
        );
 
        
        billingRecord.OverageCost = TierService.CalculateOverageCost(
            tier, 
            billingRecord.OveragePoints
        );
        
        billingRecord.TotalCost = billingRecord.BaseSubscriptionCost + billingRecord.OverageCost;
        
        // Update in database
        try
        {
            await _supabase
                .From<BillingRecordDTO>()
                .Where(x => x.Id == billingRecord.Id)
                .Update(billingRecord);
            
            _cache.TrySet(apiKeyDTO.Key, billingRecord, TimeSpan.FromDays(1));
        } catch (Exception ex)
        {
            Console.WriteLine($"Failed to fetch billing record: {ex.Message}");
        }
       
        
        // Update cache
        return billingRecord;
    }
}