using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Equipment;
using workoutAPI.Models.Position;

namespace workoutAPI.Models.Exercise;

public class ExerciseModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Instructions { get; set; }
    public string Description { get; set; }
   
    // public List<string> {get; set;}
    public PlanesSmall Plane { get; set; }
    public BodyRegionSmall BodyRegion { get; set; }
    public EquipmentSmall Equipment  { get; set; }
    public BodyMovementSmall BodyMovement { get; set; }
    public PositionSmall Position  { get; set; }
 
  
}