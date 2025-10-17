namespace workoutAPI.Headers
{
    public enum HeaderKey
    {
        // Usage (Period-level)
        UsageLimit,
        UsageTotal,
        UsageIncluded,
        UsageOverage,
        
        // Request (Request-level)
        RequestCost,
        RequestOverage,
        
        // Billing
        BillingOverageCost,
        BillingTotal,
        BillingCurrency,
        
        // Status
        WithinLimit,
        SubscriptionTier,
        SubscriptionStatus,
        
        // Rate Limiting
        RateLimitLimit,
        RateLimitRemaining,
        RateLimitReset,
        RetryAfter
    }
}