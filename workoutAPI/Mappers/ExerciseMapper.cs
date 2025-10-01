using workoutAPI.Models.Equipment;
using workoutAPI.Models.Exercise;

namespace workoutAPI.Mappers;

public class ExerciseMapper
{
    public static ExerciseModel MapFromTable(ExerciseDTO table)
    {
        return new ExerciseModel
        {
            Id = table.Id,
            Name = table.Name,
            Instructions = table.Instructions,
            Description = table.Description,
        
            Equipment = table.Equipment,
            BodyRegion = table.BodyRegions,      
            Plane = table.Planes,                
            BodyMovement = table.BodyMovements,  
            Position = table.Positions,
        };
    }
}
