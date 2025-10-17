
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models.User;

[Table("Users")]
public class UserDTO:BaseModel
{
    [PrimaryKey("id")] 
    public string Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("email")]
    public string Email { get; set; }
    
    [Column("tier")]
    public string Tier { get; set; }
    
    [Column("provider")]
    public string Provider { get; set; }
    
    [Column("provider_id")]
    public string ProviderID { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [Column("stripe_customer_id")]
    public string? StripeCustomerId { get; set; }

    [Column("stripe_subscription_id")]
    public string? StripeSubscriptionId { get; set; }

    [Column("subscription_status")]
    public string? SubscriptionStatus { get; set; }

    [Column("current_period_end")]
    public DateTime? CurrentPeriodEnd { get; set; }
}