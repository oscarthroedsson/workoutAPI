

using workoutAPI.Service;

namespace workoutAPI.Middlewear;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExerciseMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RateLimiterMiddleware>();
        builder.UseMiddleware<ApiKeyValidationMiddleware>();
        builder.UseMiddleware<PointsCalculationMiddleware>();
        builder.UseMiddleware<UsageQuotaMiddleware>();
        
        return builder;
    }
}
