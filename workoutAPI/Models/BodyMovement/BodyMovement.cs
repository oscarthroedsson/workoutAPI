using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models;

[Table("MovementType")]
public class BodyMovementDTO : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("code")]
    public string Code { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("description")]
    public string Description { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class BodyMovementModel
{
    public string Id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string createdAt { get; set; }
    public string updatedAt { get; set; }   
}

public class BodyMovementSmall
{
    public string Id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
    public string description { get; set; }
}