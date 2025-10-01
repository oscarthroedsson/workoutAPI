using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models.Position;

[Table("Positions")]
public class PositionDTO : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    public int Id { get; set; }
    
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

public class PositionModel
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } 
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}

public class PositionSmall
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } 
}