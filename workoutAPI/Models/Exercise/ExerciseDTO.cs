using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Equipment;
using workoutAPI.Models.Position;

namespace workoutAPI.Models.Exercise;

[Table("Exercises")]
public class ExerciseDTO : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("instructions")]
    public string Instructions { get; set; }
    
    [Column("description")]
    public string Description { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    
    // Relationships

    public EquipmentSmall Equipment { get; set; }
    public BodyRegionSmall BodyRegions { get; set; }      
    public PlanesSmall Planes { get; set; }               
    public BodyMovementSmall BodyMovements { get; set; } 
    
    public PositionSmall Positions { get; set; }
}

