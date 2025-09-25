using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models; 

namespace workoutAPI.Models.BodyRegions;

[Table("BodyRegions")]
public class BodyRegionsDTO : BaseModel  
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("code")]
    public string Code { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("latinName")]
    public string LatinName { get; set; }
    
    // Possible relationalshops
    [Column("muscle_regions")]
    public List<BodyRegionMuscleRelationship>? muscle_regions { get; set; }
}

public class BodyRegionMuscleRelationship
{
    [PrimaryKey("id")]
    public string Id { get; set; }
    
    [Column("muscle_id")]
    public string MuscleId { get; set; }
    
    [Column("muscles")]
    public BodyRegionMuscle? Muscles { get; set; }
}

public class BodyRegionMuscle
{
    [PrimaryKey("id")]
    public string Id { get; set; }
    
    [Column("code")]
    public string Code { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("latinName")]
    public string LatinName { get; set; }
}




