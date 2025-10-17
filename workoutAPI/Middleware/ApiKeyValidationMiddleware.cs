using System.Text.Json;
using Supabase;
using workoutAPI.Configuration;
using workoutAPI.Constants;
using workoutAPI.Models;
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
        Client supabase ,
        CacheManager cache
        )
    {
        if (
            !context.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyValue) || 
            string.IsNullOrWhiteSpace(apiKeyValue)
            )
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error("Missing API key", "MISSING_API_KEY"))
            );
            return; 
        }
        
        var apiKeyResponse = await cache.ApiKeys.GetUserApiKeyAsync(apiKeyValue!);
        if (apiKeyResponse == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error("Invalid API key"))
            );
            return;
        }
        
        if (!apiKeyResponse.IsActive)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error("API key is inactive"))
            );
            return;
        }
        
        var user = apiKeyResponse.Users;
        
        if (user == null)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error("User data not found", "USER_NOT_FOUND"))
            );
            return;
        }
        
        if (user.SubscriptionStatus == SubscriptionStatus.Canceled && user.Tier != "free")
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error(
                    "Subscription canceled. Please reactivate to continue.",
                    "SUBSCRIPTION_CANCELED"
                ))
            );
            return;
        }
        
        if (user.SubscriptionStatus == SubscriptionStatus.PastDue)
        {
            context.Response.StatusCode = 402;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error(
                    "Your payment is passed due. Please update your payment method.",
                    "PAYMENT_REQUIRED"
                ))
            );
            return;
        }
        
        if (user.SubscriptionStatus == SubscriptionStatus.Unpaid && user.Tier != "free")
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(JSONResponse.Error(
                    "Subscription unpaid. Please update payment method.",
                    "SUBSCRIPTION_UNPAID"
                ))
            );
            return;
        }
        
        Console.WriteLine($"🥅 total points: {apiKeyResponse.ReqToday}");
        context.Items["User"] = user;
        context.Items["ApiKey"] = apiKeyResponse;
        context.Items["Tier"] = user.Tier;
        
        
        await _next(context);
    }
}