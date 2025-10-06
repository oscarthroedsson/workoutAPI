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
}