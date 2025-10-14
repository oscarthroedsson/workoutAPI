using Supabase;
using workoutAPI.Models.Billing;
using workoutAPI.Models.User;
using workoutAPI.Models.ApiKey;

namespace workoutAPI.Service;

public class BillingService
{
    private readonly Client _supabase;
    
    public BillingService(Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<BillingRecordDTO> GetOrCreateBillingRecord(
        UserDTO user, 
        ApiKeyDTO apiKey, 
        string period)
    {
        var response = await _supabase
            .From<BillingRecordDTO>()
            .Where(x => x.UserId == user.Id)
            .Where(x => x.BillingPeriod == period)
            .Single();

        // Record exist → Return it
        if (response != null)   return response;

        // Create new billing record for this month
        var tierLimit = TierService.GetTierLimit(user.Tier);
        var baseCost = TierService.GetBaseCost(user.Tier);

        var newRecord = new BillingRecordDTO
        {
            UserId = user.Id,
            ApiKeyId = apiKey.Id,
            BillingPeriod = period,
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

        await _supabase.From<BillingRecordDTO>().Insert(newRecord);
        
        return newRecord;
    }

    public async Task UpsertBilling(
        BillingRecordDTO billingRecord, 
        int requestPoints, 
        string tier)
    {
        // Update totals
        billingRecord.TotalRequests += 1;
        billingRecord.TotalPointsUsed += requestPoints;

        // Calculate overage (no risk of double counting)
        billingRecord.OveragePoints = TierService.GetOveragePoints(
            billingRecord.TotalPointsUsed, 
            billingRecord.IncludedPoints
        );
 
        
        billingRecord.OverageCost = TierService.CalculateOverageCost(
            tier, 
            billingRecord.OveragePoints
        );
        billingRecord.TotalCost = billingRecord.BaseSubscriptionCost + billingRecord.OverageCost;

        // Update in database
        await _supabase
            .From<BillingRecordDTO>()
            .Where(x => x.Id == billingRecord.Id)
            .Update(billingRecord);
    }
}