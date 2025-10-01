namespace workoutAPI.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
public class Planes
{
    public Guid? Id { get; set; }   // optional
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class PlanesSmall
{
    public string? id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }  
}


[Table("Planes")]
public class Plane : BaseModel
{
    [PrimaryKey("id")]
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