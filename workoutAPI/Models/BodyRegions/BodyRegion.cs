using workoutAPI.Models.Muscle;

namespace workoutAPI.Models.BodyRegions;

public class BodyRegionModel
{
    // Base information
    public int? Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string LatinName { get; set; }
    
    // Possible Relationships
    public List<MuscleSmall> Muscles { get; set; }
    // Exercises in the future
}