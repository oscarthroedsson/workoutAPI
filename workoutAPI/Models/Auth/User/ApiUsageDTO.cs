
using Supabase.Postgrest.Attributes;

namespace workoutAPI.Models.User;

[Table("ApiUsage")]
public class ApiUsageDTO
{
    [PrimaryKey]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("api_key_id")]
    public string ApiKeyId { get; set; }
    
    [Column("endpoint")]
    public string Endpoint { get; set; }
    
    [Column("created_at")]
    public string CreatedAt { get; set; }
    
    [Column("updated_at")]
    public string ApiVersion { get; set; }
    
   
    
}