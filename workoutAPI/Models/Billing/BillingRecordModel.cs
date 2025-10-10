namespace workoutAPI.Models.Billing;

public class BillingRecordModel
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? ApiKeyId { get; set; }
    public string BillingPeriod { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int TotalPointsUsed { get; set; }
    public int IncludedPoints { get; set; }
    public int OveragePoints { get; set; }
    public decimal BaseSubscriptionCost { get; set; }
    public decimal OverageCost { get; set; }
    public decimal TotalCost { get; set; }
    public string Status { get; set; } = "pending";
    public string? StripeInvoiceId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ChargedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}