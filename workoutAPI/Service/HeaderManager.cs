using workoutAPI.Headers;
using Microsoft.AspNetCore.Http;

public class HeaderManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HeaderManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext GetContext()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            throw new InvalidOperationException("HttpContext is not available");
        return context;
    }

    private string GetHeaderName(HeaderKey key)
    {
        return key switch
        {
            // Usage (Period-level)
            HeaderKey.UsageLimit => HeaderConstants.UsageLimit,
            HeaderKey.UsageTotal => HeaderConstants.UsageTotal,
            HeaderKey.UsageIncluded => HeaderConstants.UsageIncluded,
            HeaderKey.UsageOverage => HeaderConstants.UsageOverage,
            
            // Request (Request-level)
            HeaderKey.RequestCost => HeaderConstants.RequestCost,
            HeaderKey.RequestOverage => HeaderConstants.RequestOverage,
            
            // Billing
            HeaderKey.BillingOverageCost => HeaderConstants.BillingOverageCost,
            HeaderKey.BillingTotal => HeaderConstants.BillingTotal,
            HeaderKey.BillingCurrency => HeaderConstants.BillingCurrency,
            
            // Status
            HeaderKey.WithinLimit => HeaderConstants.WithinLimit,
            HeaderKey.SubscriptionTier => HeaderConstants.SubscriptionTier,
            HeaderKey.SubscriptionStatus => HeaderConstants.SubscriptionStatus,
            
            // Rate Limiting
            HeaderKey.RateLimitLimit => HeaderConstants.RateLimitLimit,
            HeaderKey.RateLimitRemaining => HeaderConstants.RateLimitRemaining,
            HeaderKey.RateLimitReset => HeaderConstants.RateLimitReset,
            HeaderKey.RetryAfter => HeaderConstants.RetryAfter,
            
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };
    }

    public void AddHeader(HeaderKey key, string value)
    {
        try
        {
            var context = GetContext();
            
            if (context.Response.HasStarted)
                return;
            
            context.Response.Headers[GetHeaderName(key)] = value;
        }
        catch (ObjectDisposedException)
        {
            // Context was disposed, silently ignore
        }
    }

    public void AddHeaders(params (HeaderKey key, string value)[] headers)
    {
        foreach (var (key, value) in headers)
        {
            AddHeader(key, value);
        }
    }

    public void IncrementHeader(HeaderKey key, decimal valueToAdd)
    {
        try
        {
            var context = GetContext();
            if (context.Response.HasStarted) return;
        
            var headerName = GetHeaderName(key);
        
            decimal currentValue = 0m;
            if (context.Response.Headers.TryGetValue(headerName, out var existingValue))
            {
                decimal.TryParse(existingValue, out currentValue);
            }
        
            var newValue = currentValue + valueToAdd;
            context.Response.Headers[headerName] = newValue.ToString();
        }
        catch (ObjectDisposedException)
        {
            // Context disposed, silently ignore
        }
    }
}