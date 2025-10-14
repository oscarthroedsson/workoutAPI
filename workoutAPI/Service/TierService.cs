using workoutAPI.Configuration;

namespace workoutAPI.Service;

public static class TierService
{
    
    public static Dictionary<string, int> GetAllTierLimits()
    {
        return new Dictionary<string, int>
        {
            { Tiers.FREE, Tiers.FreeLimit },
            { Tiers.HOBBY, Tiers.HobbyLimit },
            { Tiers.STARTUP, Tiers.StartUpLimit },
            { Tiers.BUSINESS, Tiers.BusniessLimit }
        };
    }

    public static Dictionary<string, int> GetAllTierRateLimits()
    {
        return new Dictionary<string, int>
        {
            { Tiers.FREE, Tiers.FreeMaxReqPerSecond },
            { Tiers.HOBBY, Tiers.HobbyMaxReqPerSecond },
            { Tiers.STARTUP, Tiers.StartUpMaxReqPerSecond },
            { Tiers.BUSINESS, Tiers.BusniessMaxReqPerSecond }
        };
    }

    public static int GetTierRateLimit(string tier)
    {
        return tier.ToLowerInvariant() switch
        {
            Tiers.FREE => Tiers.FreeMaxReqPerSecond,
            Tiers.HOBBY => Tiers.HobbyMaxReqPerSecond,
            Tiers.STARTUP => Tiers.StartUpMaxReqPerSecond,
            Tiers.BUSINESS => Tiers.BusniessMaxReqPerSecond,
            _ => 0
        };
    }
    public static int GetTierLimit(string tier)
    {
        return tier.ToLowerInvariant() switch
        {
            Tiers.FREE => Tiers.FreeLimit,
            Tiers.HOBBY => Tiers.HobbyLimit,
            Tiers.STARTUP => Tiers.StartUpLimit,
            Tiers.BUSINESS => Tiers.BusniessLimit,
            _ => 0
        };
    }
    public static decimal GetOveragePrice(string tier)
    {
        return tier.ToLower() switch
        {
            Tiers.HOBBY => Tiers.HobbyExtraCostPerPoint,      // $0.006 / point
            Tiers.STARTUP => Tiers.HobbyExtraCostPerPoint,    // $0.005 / point
            Tiers.BUSINESS => Tiers.HobbyExtraCostPerPoint,   // $0.004 / point
            _ => 0m                     // FREE has no overage pricing
        };
    }
    public static decimal GetBaseCost(string tier)
    {
        return tier.ToLower() switch
        {
            Tiers.FREE => 0m,
            Tiers.HOBBY => 29m,
            Tiers.STARTUP => 79m,
            Tiers.BUSINESS => 200m,
            _ => 0m
        };
    }
    public static bool IsValidTier(string tier)
    {
        return tier.ToLower() switch
        {
            Tiers.FREE => true,
            Tiers.HOBBY => true,
            Tiers.STARTUP => true,
            Tiers.BUSINESS => true,
            _ => false
        };
    }
    
    public static bool ShouldAllowRequest(string tier, int currentUsage, int requestPoints)
    {
        tier = tier.ToLower();
        var limit = GetTierLimit(tier);
        if(tier == Tiers.FREE && currentUsage >= limit) return false;
        return true;
    }
    
    public static bool ShouldBeBilled(string tier, int currentUsage, decimal requestPoints)
    {
        var limit = GetTierLimit(tier);
        if(tier == Tiers.FREE) return false;
        
        return currentUsage + requestPoints > limit;
    }
    
    public static decimal GetOveragePoints(decimal totalPointsUsed, decimal includedPoints)
    {
        return Math.Max(0, totalPointsUsed - includedPoints);
    }
    
    public static decimal CalculateOverageCost(string tier, decimal overagePoints)
    {
        if (tier.ToLower() == Tiers.FREE.ToLower()) 
            return 0m;
    
        var pricePerPoint = GetOveragePrice(tier);
        return overagePoints * pricePerPoint;
    }
    
}

