using workoutAPI.Models.Equipment;
using workoutAPI.Models.Exercise;

namespace workoutAPI.Mappers;

public class ExerciseMapper
{
    public static ExerciseModel MapFromTable(ExerciseDTO table)
    {
        return new ExerciseModel
        {
            // Base
            Id = table.Id,
            Name = table.Name,
            Equipment = table.Equipment,
            BodyRegion = table.BodyRegions,  
            Position = table.Positions,
         
            // Extras
            Instructions = table.Instructions,
            Description = table.Description,
            Plane = table.Planes,                
            BodyMovement = table.BodyMovements,  
        };
    }
}
