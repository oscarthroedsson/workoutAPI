namespace workoutAPI.Headers
{
    public static class HeaderConstants
    {
        // Usage (Period-level)
        public const string UsageLimit = "X-Usage-Limit";
        public const string UsageTotal = "X-Usage-Total";
        public const string UsageIncluded = "X-Usage-Included";
        public const string UsageOverage = "X-Usage-Overage";
        
        // Request (Request-level)
        public const string RequestCost = "X-Request-Cost";
        public const string RequestOverage = "X-Request-Overage";
        
        // Billing
        public const string BillingOverageCost = "X-Billing-Overage-Cost";
        public const string BillingTotal = "X-Billing-Total";
        public const string BillingCurrency = "X-Billing-Currency";
        
        // Status
        public const string WithinLimit = "X-Within-Limit";
        public const string SubscriptionTier = "X-Subscription-Tier";
        public const string SubscriptionStatus = "X-Subscription-Status";
        
        // Rate Limiting
        public const string RateLimitLimit = "X-RateLimit-Limit";
        public const string RateLimitRemaining = "X-RateLimit-Remaining";
        public const string RateLimitReset = "X-RateLimit-Reset";
        public const string RetryAfter = "X-Retry-After";
    }
}