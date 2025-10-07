

namespace workoutAPI.Middlewear;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExerciseMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ApiKeyValidationMiddleware>();
        return builder;
    }
    
    
    
}