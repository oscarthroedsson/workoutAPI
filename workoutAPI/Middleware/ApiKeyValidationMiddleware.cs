using System.Text.Json;
using Supabase;
using workoutAPI.Models;
using workoutAPI.Models.ApiKey;
using workoutAPI.Models.User;

namespace workoutAPI.Middlewear;

public class ApiKeyValidationMiddleware
{
    private readonly RequestDelegate _next;
    
    
    public ApiKeyValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(
        HttpContext context, 
        Client supabase)
    {
        
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyValue))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error("Missing API key", "MISSING_API_KEY", "Missing API key"))
            );
            return; 
        }
        
        var response = await supabase
            .From<ApiKeyDTO>()
            .Select("*")
            .Where(x => x.Key == apiKeyValue)
            .Single();
        
        
        if (response == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error("Invalid API key"))
            );
            return;
        }
        
        if (!response.IsActive)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error("API key is inactive"))
            );
            return;
        }
        
        var user = await supabase.From<UserDTO>().Where(x => x.Id == response.UserID).Single();
        
        context.Items["User"] = user;
        context.Items["ApiKey"] = apiKeyValue.ToString();
        context.Items["Tier"] = user.Tier;
        
        await _next(context);
    }
}