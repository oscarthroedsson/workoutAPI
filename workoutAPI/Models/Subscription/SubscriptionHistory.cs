using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models.User;

[Table("SubscriptionHistory")]
public class SubscriptionHistory : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;
    
    [Column("user_id")]
    public string? UserId { get; set; }
    
    [Column("stripe_event_id")]
    public string StripeEventId { get; set; } = string.Empty;
    
    [Column("event_type")]
    public string EventType { get; set; } = string.Empty;
    
    [Column("subscription_status")]
    public string? SubscriptionStatus { get; set; }
    
    [Column("amount")]
    public decimal? Amount { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}