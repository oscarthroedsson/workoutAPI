using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models.ApiKey;

[Table("ApiKeys")]
public class ApiKeyDTO :BaseModel
{
    [PrimaryKey]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("user_id")]
    public string UserId { get; set; }
    
    [Column("key")]
    public string Key { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Column("req_today")]
    public int ReqToday { get; set; }
    
    [Column("last_reset_date")]
    public DateTime LastResetDate { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
}