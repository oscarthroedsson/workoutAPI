using System.ComponentModel.DataAnnotations;
using workoutAPI.Enums;

namespace workoutAPI.Models.Requests
{
    public class GetExercisesRequest 
    {
        
        // Filters
        public string? BodyRegion { get; set; }
        public string? Position { get; set; }
        public string? Plane { get; set; }
        public string? BodyMovement { get; set; }
        
        // includes
        public bool IncludeDetail { get; set; } = false;
        public bool IncludeInstruction { get; set; } = false;
        public bool IncludeDescription { get; set; } = false;
        public bool IncludePlane { get; set; } = false;
        public bool IncludeBodyMovement { get; set; } = false;
        
        // Pagination
        [Range(0, int.MaxValue, ErrorMessage = "Offset must be >= 0")]
        public int Offset { get; set; } = 0;
        
        [Range(1, 100, ErrorMessage = "Number must be between 1 and 100")]
        public int Number { get; set; } = 20;
        
        [RegularExpression("^(name|created_at|updated_at)$", 
            ErrorMessage = "Sort must be name, created_at, or updated_at")]
        public string Sort { get; set; } = "name";
        
        [RegularExpression("^(asc|desc)$", 
            ErrorMessage = "Order must be asc or desc")]
        public string Order { get; set; } = "asc";
    }
    
  
}