using workoutAPI.Models.User;
using Stripe;
using workoutAPI.Models.Billing;

namespace workoutAPI.Services;

public class SubscriptionWebhookHandler
{
    private readonly Supabase.Client _supabase;

    public SubscriptionWebhookHandler(Supabase.Client supabase)
    {
        _supabase = supabase;
    }

    public async Task HandleCheckoutSessionCompleted(Event stripeEvent)
    {
        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
        if (session == null) return;

        var user = await _supabase
            .From<UserDTO>()
            .Where(x => x.StripeCustomerId == session.CustomerId)
            .Single();

        if (user != null && !string.IsNullOrEmpty(session.SubscriptionId))
        {
            user.StripeSubscriptionId = session.SubscriptionId;
            await _supabase.From<UserDTO>().Update(user);
        }
    }

    public async Task HandleSubscriptionUpdated(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        if (subscription == null) return;

        var user = await _supabase
            .From<UserDTO>()
            .Where(x => x.StripeCustomerId == subscription.CustomerId)
            .Single();

        if (user != null)
        {
            user.StripeSubscriptionId = subscription.Id;
            user.SubscriptionStatus = subscription.Status;
            user.CurrentPeriodEnd = subscription.BillingCycleAnchor;

            var priceId = subscription.Items.Data[0].Price.Id;
            user.Tier = await GetTierFromPriceId(priceId);

            await _supabase.From<UserDTO>().Update(user);
        }
    }

    public async Task HandleSubscriptionDeleted(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        if (subscription == null) return;

        var user = await _supabase
            .From<UserDTO>()
            .Where(x => x.StripeCustomerId == subscription.CustomerId)
            .Single();

        if (user != null)
        {
            user.Tier = "free";
            user.SubscriptionStatus = "canceled";
            user.StripeSubscriptionId = null;
            await _supabase.From<UserDTO>().Update(user);
        }
    }

    public async Task HandleInvoicePaid(Event stripeEvent)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        if (invoice == null || string.IsNullOrEmpty(invoice.CustomerId)) return;

        // Find user
        var user = await _supabase
            .From<UserDTO>()
            .Where(x => x.StripeCustomerId == invoice.CustomerId)
            .Single();

        if (user == null) return;

        // Get current billing period
    
        // Update BillingRecord status to "paid"
        var billingRecord = await _supabase
            .From<BillingRecordDTO>()
            .Where(x => x.UserId == user.Id)
            .Where(x => x.BillingPeriod == DateTime.UtcNow.ToString("yyyy-MM"))
            .Single();

        if (billingRecord != null)
        {
            billingRecord.Status = "paid";
            billingRecord.StripeInvoiceId = invoice.Id;
            billingRecord.ChargedAt = DateTime.UtcNow;
        
            await _supabase.From<BillingRecordDTO>().Update(billingRecord);
        }
    }

    public async Task HandleInvoicePaymentFailed(Event stripeEvent)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        if (invoice == null) return;

        var user = await _supabase
            .From<UserDTO>()
            .Where(x => x.StripeCustomerId == invoice.CustomerId)
            .Single();

        if (user != null)
        {
            user.SubscriptionStatus = "past_due";
            await _supabase.From<UserDTO>().Update(user);
        }
    }

    private async Task<string> GetTierFromPriceId(string priceId)
    {
        var priceService = new PriceService();
        var price = await priceService.GetAsync(priceId);

        if (price.LookupKey != null)
        {
            if (price.LookupKey.Contains("hobby")) return "hobby";
            if (price.LookupKey.Contains("startup")) return "startup";
            if (price.LookupKey.Contains("business")) return "business";
        }

        return "free";
    }
    
    public string? GetCustomerIdFromEvent(Stripe.Event stripeEvent)
    {
        return stripeEvent.Type switch
        {
            "checkout.session.completed" => (stripeEvent.Data.Object as Stripe.Checkout.Session)?.CustomerId,
            "customer.subscription.created" or "customer.subscription.updated" or "customer.subscription.deleted" 
                => (stripeEvent.Data.Object as Stripe.Subscription)?.CustomerId,
            "invoice.paid" or "invoice.payment_failed" 
                => (stripeEvent.Data.Object as Stripe.Invoice)?.CustomerId,
            _ => null
        };
    }
}