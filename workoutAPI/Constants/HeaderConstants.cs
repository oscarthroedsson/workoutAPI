using workoutAPI.Headers;

public static class HeaderConstants
{
    public const string QuotaRequested = "X-Quota-Requested";
    public const string QuotaUsed = "X-Quota-Used";
    public const string QuotaExceeded = "X-Quota-Exceeded";
    public const string OveragePoints = "X-Quota-Overage";
    public const string OverageCost = "X-Quota-Overage-Cost";
    public const string RetryAfter = "X-Retry-After";
    
}