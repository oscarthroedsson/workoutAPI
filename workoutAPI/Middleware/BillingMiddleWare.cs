using workoutAPI.Configuration;
using workoutAPI.Headers;
using workoutAPI.Models.ApiKey;
using workoutAPI.Models.Billing;
using workoutAPI.Models.User;
using workoutAPI.Service;
using workoutAPI.Services;

namespace workoutAPI.Middlewear;

public class BillingMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly StripeService _stripeService;
    
    public BillingMiddleWare(
        RequestDelegate next,
        StripeService  stripeService
        )
    {
        _next = next;
        _stripeService = stripeService;
    }

    public async Task InvokeAsync(HttpContext context, BillingService billingService, HeaderManager headerManager)
    {
        
        await _next(context);
        // Server crash, dont bill
        if (context.Response.StatusCode >= 500) return;
        // <400 should be billed, it is a client error
        
        // === AFTER CONTROLLER === 
        bool shouldBeBilled = context.Items.TryGetValue("ShouldBeBilled", out var value) && value is bool b && b;
        var tier = (string)context.Items["Tier"];
        
        
        if (
            !shouldBeBilled ||
            tier == Tiers.FREE.ToLowerInvariant() // Free can never be billed
            ) return;
        
        var apiKeyDto = context.Items["ApiKey"] as ApiKeyDTO;
        var userDTO = context.Items["User"] as UserDTO;
        var requestPoints = (decimal)(context.Items["RequestPoints"] ?? 0m);
        decimal totalPointUsed = (decimal)(context.Items["TotalPointsUsed"] ?? 0m);
        var currentPeriod = DateTime.UtcNow;
        
        // Create/Update current billingPeriod 
        BillingRecordDTO billingRecord = await billingService.UpsertBilling(userDTO, apiKeyDto, currentPeriod,requestPoints,tier);
        var billingPoints = TierService.GetRequestOveragePoints(totalPointUsed, requestPoints, tier);
        
       
        
        // Fire-and-forget Stripe report
        _ = Task.Run(async () =>
        {
            try
            {
                if (billingPoints > 0 && !string.IsNullOrEmpty(userDTO.StripeCustomerId))
                {
                    await _stripeService.ReportUsageAsync(
                        quantity: (long)billingPoints,
                        customerId: userDTO.StripeCustomerId
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Stripe report failed: {ex.Message}");
            }
        });
        
        
  
    }
    
}