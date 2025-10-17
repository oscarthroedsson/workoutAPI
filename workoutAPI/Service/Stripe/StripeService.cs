using Stripe;
using Stripe.Checkout;

namespace workoutAPI.Services;

public class StripeService
{
    private readonly string _secretKey;
    
    public StripeService(IConfiguration configuration)
    {
        _secretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") 
                     ?? throw new ArgumentNullException("STRIPE_SECRET_KEY is not configured");
    
        Console.WriteLine($"✅ Stripe API Key loaded: {_secretKey.Substring(0, 10)}...");
        StripeConfiguration.ApiKey = _secretKey;
    }
    
   
    public async Task<Customer> CreateCustomerAsync(string email, string name)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = name,
            Metadata = new Dictionary<string, string>
            {
                { "source", "workout_api" }
            }
        };
        
        var service = new CustomerService();
        return await service.CreateAsync(options);
    }
    
  
    public async Task<Session> CreateCheckoutSessionAsync(
        string customerId, 
        string priceId, 
        string successUrl, 
        string cancelUrl)
    {
        StripeConfiguration.ApiKey = _secretKey;
        Console.WriteLine($"🔑 Creating checkout with API key: {_secretKey.Substring(0, 10)}...");
        
        var options = new SessionCreateOptions
        {
            Customer = customerId,
            Mode = "subscription",
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = 1
                }
            },
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };
        
        var service = new SessionService();
        return await service.CreateAsync(options);
    }
    

    public async Task<Subscription> GetSubscriptionAsync(string subscriptionId)
    {
        var service = new SubscriptionService();
        return await service.GetAsync(subscriptionId);
    }
    
 
    public async Task<Subscription> CancelSubscriptionAsync(string subscriptionId)
    {
        var service = new SubscriptionService();
        return await service.CancelAsync(subscriptionId);
    }
    
   
    public async Task ReportUsageAsync(long quantity, string customerId)
    {
        var options = new Stripe.Billing.MeterEventCreateOptions
        {
            EventName = "point_usage",
            Payload = new Dictionary<string, string>
            {
                { "value", quantity.ToString() },
                { "stripe_customer_id", customerId }
            }
        };
        
        var service = new Stripe.Billing.MeterEventService();
        await service.CreateAsync(options);
    }
    

    public async Task<Stripe.BillingPortal.Session> CreatePortalSessionAsync(
        string customerId, 
        string returnUrl
        )
    {
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = customerId,
            ReturnUrl = returnUrl
        };
        
        var service = new Stripe.BillingPortal.SessionService();
        return await service.CreateAsync(options);
    }
}