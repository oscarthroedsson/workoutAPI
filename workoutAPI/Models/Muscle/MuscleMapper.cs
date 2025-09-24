using workoutAPI.Models;
using workoutAPI.Models.Muscle;

public static class MuscleMapper
{
    public static Muscle MapFromTable(MuscleTable table)
    {
        return new Muscle
        {
            Id = table.Id,
            Code = table.Code,
            Name = table.Name,
            LatinName = table.LatinName,
            CreatedAt = table.CreatedAt,
            UpdatedAt = table.UpdatedAt,

            Planes = table.muscle_actions_planes?.Select(p => new PlanesSmall
            {
                Name = p.Planes.Name,
                Description = p.Planes.Description
            }).ToList(),

            Actions = table.muscle_actions_joint?.Select(j => new ActionSmall
            {
                Name = j.Joint.Name,
                Description = j.Joint.LatinName,
                Actions = new PlanesSmall
                {
                    Name = j.Actions.Name,
                    Description = j.Actions.Description
                }
            }).ToList()
        };
        
    }

}