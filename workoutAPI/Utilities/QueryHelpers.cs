using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Muscle;

namespace workoutAPI.Services;

public class QueryHelpers
{
    
    
    public static SupabaseQueryBuilder DefaultQueryMuscle(MuscleQueryOptions options = null)
    {
        options ??= new MuscleQueryOptions();
        return new SupabaseQueryBuilder()
            .StartWith("id", "code", "name", "latinName", "created_at", "updated_at")
            .AddIf(options.IncludePlaneMovement,
                "muscle_actions_planes:muscle_actions_planes_muscle_id_fkey(plane_id, Planes(name, description))")
            .AddIf(options.IncludeMuscleRegion,
                "muscle_regions:muscle_regions_muscle_id_fkey(region_id, BodyRegions!muscle_regions_region_id_fkey(name, latinName))")
            .AddIf(options.IncludeJointActions,
                "muscle_actions_joint:muscle_actions_muscle_id_fkey(joint_id, action_id, Joint(name, latinName), Actions(name, description))");
    }
    
    public static SupabaseQueryBuilder DefaultQueryBodyRegion(BodyRegionQueryOptions options = null)
    {
        return new SupabaseQueryBuilder()
            .StartWith("id", "code", "name", "latinName")
            .AddIf(options.IncludeMuscles, " muscle_regions:muscle_regions_region_id_fkey(id, muscle_id, muscles:muscle_regions_muscle_id_fkey(id, code, name, latinName))");

    }

    public static List<string> FilterValidQueryParams(params string[] queryParams)
    {
        return queryParams
            .Where(param => !string.IsNullOrWhiteSpace(param)) // ej null eller tom
            .Where(param => !double.IsNaN(ConvertToDouble(param))) // ej NaN
            .Where(param => !double.IsInfinity(ConvertToDouble(param))) // ej infinity
            .ToList();
    }

    private static double ConvertToDouble(string param)
    {
        return double.TryParse(param, out var number) ? number : double.NaN;
    }
    
}

