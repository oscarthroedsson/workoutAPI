namespace workoutAPI.Models.Exercise;

public class ExerciseQueryOptions
{
    public bool IncludeDetail { get; set; } = false;
    public bool IncludeInstruction { get; set; } = false;
    public bool IncludeDescription { get; set; } = false;
    public bool IncludePlane { get; set; } = false;
    public bool IncludeBodyMovement { get; set; } = false;
}