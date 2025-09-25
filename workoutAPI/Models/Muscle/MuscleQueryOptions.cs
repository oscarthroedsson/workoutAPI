namespace workoutAPI.Models.Muscle;

public class MuscleQueryOptions
{
  
        public bool IncludePlaneMovement { get; set; } = false;
        public bool IncludeMuscleRegion { get; set; } = false;
        public bool IncludeJointActions { get; set; } = false;
        public bool IncludeMovementType { get; set; } = false;
    
}