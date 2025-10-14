
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models.User;

[Table("ApiUsage")]
public class ApiUsageDTO : BaseModel
{
    [PrimaryKey]
    [Column("id")]

    public string Id { get; set; }
    
    [Column("api_key_id")]
    public string ApiKeyId { get; set; }
    
    [Column("endpoint")]
    public string Endpoint { get; set; }
    
    [Column("point_costs")]
    public decimal PointCost { get; set; }
    
    [Column("created_at")]

    public string CreatedAt { get; set; }
    
    [Column("updated_at")]
    public string UpdatedAt { get; set; }

    public bool ShouldSerializeId() => Id != null;
    public bool ShouldSerializeCreatedAt() => CreatedAt != null;
    public bool ShouldSerializeUpdatedAt() => UpdatedAt != null;

}