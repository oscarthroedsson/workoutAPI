using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models.ApiKey;

[Table("api_key_user_info")]
public class ApiKeyUserInfo : BaseModel
{
    [Column("api_key_id")]
    public string ApiKeyId { get; set; }
    
    [Column("api_key")]
    public string ApiKey { get; set; }
    
    [Column("key_name")]
    public string KeyName { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Column("req_today")]
    public int ReqToday { get; set; }
    
    [Column("last_reset_date")]
    public string LastResetDate { get; set; }
    
    [Column("user_id")]
    public string UserId { get; set; }
    
    [Column("email")]
    public string Email { get; set; }
    
    [Column("tier")]
    public string Tier { get; set; }
    
    [Column("user_name")]
    public string UserName { get; set; }
    
    [Column("provider")]
    public string Provider { get; set; }
}