using Supabase.Postgrest;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Muscle;

namespace workoutAPI.Mappers;

public class BodyRegionMapper
{
    public static BodyRegionModel MapFromTable(BodyRegionsDTO table)
    {
        return new BodyRegionModel
        {
            Id = table.Id,
            Code = table.Code,
            Name = table.Name,
            LatinName = table.LatinName,
            
            Muscles = table.muscle_regions.Select(m => new MuscleSmall
            {
                Code = m.Muscles.Code,
                Name = m.Muscles.Name,
                LatinName = m.Muscles.LatinName
                
            }).ToList()
        };

    }
}