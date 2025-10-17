using Microsoft.AspNetCore.Mvc;
using workoutAPI.Models.Billing;
using workoutAPI.Services;
using workoutAPI.Models.User;
using workoutAPI.Service;

namespace workoutAPI.Controllers;

[ApiController]
[Route("api/subscription")]
public class SubscriptionController : ControllerBase
{
    private readonly StripeService _stripeService;
    private readonly Supabase.Client _supabase;
    private readonly SubscriptionWebhookHandler _webhookHandler;

    public SubscriptionController(
        StripeService stripeService,
        Supabase.Client supabase,
        SubscriptionWebhookHandler webhookHandler)
    {
        _stripeService = stripeService;
        _supabase = supabase;
        _webhookHandler = webhookHandler;
    }

   
    [HttpGet("checkout")]
    public async Task<IActionResult> CreateCheckoutSession([FromQuery] string plan)
    {
        try
        {
            var userId = Request.Headers["X-User-Id"].ToString();
            if (string.IsNullOrEmpty(userId)) return BadRequest("User ID is required");
            
            var userResponse = await _supabase
                .From<UserDTO>()
                .Where(x => x.Id == userId)
                .Single();
            if (userResponse == null) return NotFound("User not found");
           

            string stripeCustomerId = userResponse.StripeCustomerId;
            if (string.IsNullOrEmpty(stripeCustomerId))
            {
                var customer = await _stripeService.CreateCustomerAsync(
                    userResponse.Email,
                    userResponse.Name
                );
                stripeCustomerId = customer.Id;

                userResponse.StripeCustomerId = stripeCustomerId;
                await _supabase.From<UserDTO>().Update(userResponse);
            }


            if (!TierService.IsValidTier(plan)) throw new ArgumentException("Invalid plan");
            string lookupKey = plan.ToLower() switch
            {
                "hobby" => "hobby_monthly",
                "startup" => "startup_monthly",
                "business" => "business_monthly",
            };

            var priceService = new Stripe.PriceService();
            var prices = await priceService.ListAsync(new Stripe.PriceListOptions
            {
                LookupKeys = new List<string> { lookupKey }
            });

            if (prices.Data.Count == 0) return BadRequest("Price not found for plan");
            var priceId = prices.Data[0].Id;

            var successUrl = $"{Request.Scheme}://{Request.Host}/subscription/success";
            var cancelUrl = $"{Request.Scheme}://{Request.Host}/subscription/cancel";

            var session = await _stripeService.CreateCheckoutSessionAsync(
                stripeCustomerId,
                priceId,
                successUrl,
                cancelUrl
            );

            return Ok(new { url = session.Url });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

   
    [HttpGet("portal")]
    public async Task<IActionResult> CreatePortalSession()
    {
        try
        {
            var userId = Request.Headers["X-User-Id"].ToString();
            if (string.IsNullOrEmpty(userId)) return BadRequest("User ID is required");
            

            var userResponse = await _supabase
                .From<UserDTO>()
                .Where(x => x.Id == userId)
                .Single();

            if (userResponse == null || string.IsNullOrEmpty(userResponse.StripeCustomerId))
            {
                return BadRequest("User has no Stripe customer");
            }

            var returnUrl = $"{Request.Scheme}://{Request.Host}/account";
            var session = await _stripeService.CreatePortalSessionAsync(
                userResponse.StripeCustomerId,
                returnUrl
            );

            return Ok(new { url = session.Url });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

   
    [HttpGet("status")]
    public async Task<IActionResult> GetSubscriptionStatus()
    {
        try
        {
            var userId = Request.Headers["X-User-Id"].ToString();
            if (string.IsNullOrEmpty(userId)) return BadRequest("User ID is required");
            var currentPeriod = DateTime.UtcNow.ToString("yyyy-MM");
            
            
            var userTask = _supabase.From<UserDTO>()
                .Where(x => x.Id == userId)
                .Single();

            var billingTask = _supabase.From<BillingRecordDTO>()
                .Where(x => x.UserId == userId)
                .Where(x => x.BillingPeriod == currentPeriod)
                .Single();
            
            await Task.WhenAll(userTask, billingTask);

            var userResponse = await userTask;
            var billingRecord = await billingTask;
            if (userResponse == null) return NotFound("User not found");

            var response = new
            {
                tier = userResponse.Tier,
                subscriptionStatus = userResponse.SubscriptionStatus,
                currentPeriodEnd = userResponse.CurrentPeriodEnd,
                overagePoints = billingRecord?.OveragePoints ?? 0,
                overageCost = billingRecord?.OverageCost ?? 0m,
                totalCost = billingRecord?.TotalCost ?? 0m
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

   
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();
            var webhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");

            if (string.IsNullOrEmpty(webhookSecret)) return StatusCode(500, "Webhook secret not configured");

            Stripe.Event stripeEvent;
            try
            {
                stripeEvent = Stripe.EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook signature verification failed: {ex.Message}");
            }

            var existingEvent = await _supabase
                .From<SubscriptionHistory>()
                .Where(x => x.StripeEventId == stripeEvent.Id)
                .Single();

            if (existingEvent != null) return Ok("Event already processed");
            

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    await _webhookHandler.HandleCheckoutSessionCompleted(stripeEvent);
                    break;
                case "customer.subscription.created":
                case "customer.subscription.updated":
                    await _webhookHandler.HandleSubscriptionUpdated(stripeEvent);
                    break;
                case "customer.subscription.deleted":
                    await _webhookHandler.HandleSubscriptionDeleted(stripeEvent);
                    break;
                case "invoice.paid":
                    await _webhookHandler.HandleInvoicePaid(stripeEvent);
                    break;
                case "invoice.payment_failed":
                    await _webhookHandler.HandleInvoicePaymentFailed(stripeEvent);
                    break;
                default:
                    Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                    break;
            }
            
            string? userId = null;
            var customerId = _webhookHandler.GetCustomerIdFromEvent(stripeEvent);
            if (!string.IsNullOrEmpty(customerId))
            {
                var user = await _supabase
                    .From<UserDTO>()
                    .Where(x => x.StripeCustomerId == customerId)
                    .Single();
                userId = user?.Id;
            }

            var historyEntry = new SubscriptionHistory
            {
                UserId = userId,
                StripeEventId = stripeEvent.Id,
                EventType = stripeEvent.Type,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase.From<SubscriptionHistory>().Insert(historyEntry);

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Webhook error: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
}