namespace workoutAPI.Models.RateLimiter;

public class RateLimitEntry
{
    public int Amount { get; set; }    
    public DateTime Expiry { get; set; } 
}
