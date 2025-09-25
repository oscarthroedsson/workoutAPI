using Supabase.Postgrest;
using workoutAPI.Models.BodyRegions;

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
            
            Muscles = []
        };

    }
}