using workoutAPI.Headers;

public static class HeaderConstants
{
    public const string QuotaExceeded = "X-Quota-Exceeded";
    public const string OveragePoints = "X-Overage-Points";
    public const string OverageCost = "X-Overage-Cost";
    public const string WithinLimit = "X-Within-Limit";
    public const string PointsRequested = "X-Points-Requested";

    public static string GetHeaderName(HeaderKey key) => key switch
    {
        HeaderKey.QuotaExceeded => QuotaExceeded,
        HeaderKey.OveragePoints => OveragePoints,
        HeaderKey.OverageCost => OverageCost,
        HeaderKey.WithinLimit => WithinLimit,
        HeaderKey.PointsRequested => PointsRequested,
        _ => throw new ArgumentOutOfRangeException(nameof(key), $"No header defined for {key}")
    };
}