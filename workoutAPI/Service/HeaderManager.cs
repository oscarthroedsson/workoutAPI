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
            HeaderKey.OveragePoints => HeaderConstants.OveragePoints,
            HeaderKey.OverageCost => HeaderConstants.OverageCost,
            HeaderKey.WithinLimit => HeaderConstants.WithinLimit,
            HeaderKey.PointsRequested => HeaderConstants.PointsRequested,
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

    public void SetQuotaExceeded(bool exceeded) =>
        AddHeader(HeaderKey.QuotaExceeded, exceeded.ToString().ToLowerInvariant());

    public void SetOveragePoints(int points) =>
        AddHeader(HeaderKey.OveragePoints, points.ToString());

    public void SetOverageCost(decimal cost) =>
        AddHeader(HeaderKey.OverageCost, $"${cost:F2}");

    public void SetWithinLimit(bool withinLimit) =>
        AddHeader(HeaderKey.WithinLimit, withinLimit.ToString().ToLowerInvariant());

    public void SetPointsRequested(int points) =>
        AddHeader(HeaderKey.PointsRequested, points.ToString());
}