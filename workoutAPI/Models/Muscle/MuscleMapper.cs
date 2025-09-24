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
            MuscleRegion = table.muscle_regions?
                               .Select(br => br.BodyRegions?.Name)
                               .Where(name => !string.IsNullOrEmpty(name))
                               .Distinct()
                               .DefaultIfEmpty() 
                               .Aggregate((current, next) => 
                                   string.IsNullOrEmpty(current) ? next : current + ", " + next) 
                           ?? string.Empty,
            Planes = table.muscle_actions_planes?.Select(p => new PlanesSmall
            {
                Name = p.Planes.Name,
                Description = p.Planes.Description
            }).ToList(),

            Actions = table.muscle_actions_joint?.Select(j => new ActionSmall
            {
                Name = j.Joint.Name,
                LatinName = j.Joint.LatinName,
                Actions = new PlanesSmall
                {
                    Name = j.Actions.Name,
                    Description = j.Actions.Description
                }
            }).ToList()
        };
        
    }

}