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
            HeaderKey.QuotaExceeded => HeaderConstants.QuotaExceeded,
            HeaderKey.QuotaRequested => HeaderConstants.QuotaRequested,
            HeaderKey.OveragePoints => HeaderConstants.OveragePoints,
            HeaderKey.OverageCost => HeaderConstants.OverageCost,
            HeaderKey.QuotaUsed => HeaderConstants.QuotaUsed,
            HeaderKey.RetryAfter => HeaderConstants.RetryAfter,
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };
    }

    public void AddHeader(HeaderKey key, string value)
    {
        try
        {
            var context = GetContext();
            
            // Don't add headers if response has already started
            if (context.Response.HasStarted)
                return;
            
            context.Response.Headers[GetHeaderName(key)] = value;
        }
        catch (ObjectDisposedException)
        {
            // Context was disposed, silently ignore
            // This can happen if middleware tries to add headers after response is sent
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
        
            // Hämta nuvarande värde
            decimal currentValue = 0m;
            if (context.Response.Headers.TryGetValue(headerName, out var existingValue))
            {
                decimal.TryParse(existingValue, out currentValue);
            }
        
            // Lägg ihop
            var newValue = currentValue + valueToAdd;
        
            // Sätt uppdaterat värde
            context.Response.Headers[headerName] = newValue.ToString();
        }
        catch (ObjectDisposedException)
        {
            // Context disposed, silently ignore
        }
    }
 
}