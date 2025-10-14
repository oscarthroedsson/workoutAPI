using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models.Billing;

[Table("BillingRecords")]
public class BillingRecordDTO : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;
    
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [Column("api_key_id")]
    public string? ApiKeyId { get; set; }
    
    [Column("billing_period")]
    public string BillingPeriod { get; set; } = string.Empty;
    
    [Column("total_requests")]
    public int TotalRequests { get; set; }
    
    [Column("total_points_used")]
    public int TotalPointsUsed { get; set; }
    
    [Column("included_points")]
    public int IncludedPoints { get; set; }
    
    [Column("overage_points")]
    public decimal OveragePoints { get; set; }
    
    [Column("base_subscription_cost")]
    public decimal BaseSubscriptionCost { get; set; }
    
    [Column("overage_cost")]
    public decimal OverageCost { get; set; }
    
    [Column("total_cost")]
    public decimal TotalCost { get; set; }
    
    [Column("status")]
    public string Status { get; set; } = "active";
    
    [Column("stripe_invoice_id")]
    public string? StripeInvoiceId { get; set; }
    
    [Column("stripe_payment_intent_id")]
    public string? StripePaymentIntentId { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("charged_at")]
    public DateTime? ChargedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}