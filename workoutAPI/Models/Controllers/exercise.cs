namespace workoutAPI.Models.Controllers
{
    public class ExerciseParams
    {
        public string? BodyRegion { get; set; }
        public string? Position { get; set; }
        public string? Plane { get; set; }
        public bool IncludeDetails { get; set; } = false;
        public bool IncludeInstructions { get; set; } = false;
    }
}