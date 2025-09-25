using workoutAPI.Models.Muscle;









public class SupabaseQueryBuilder
{
    private List<string> _fields = new List<string>();
    
    public SupabaseQueryBuilder StartWith(params string[] baseFields)
    {
        _fields.AddRange(baseFields);
        return this; // make it possible to chain functions
    }
    
    public SupabaseQueryBuilder AddIf(bool include, string relationQuery)
    {
        if (include) _fields.Add(relationQuery);
        return this; // make it possible to chain functions
    }

    public SupabaseQueryBuilder Add(string baseFields)
    {
        _fields.Add(baseFields);
        return this;    // make it possible to chain functions
    }


    public static SupabaseQueryBuilder MuscleDefaultQuery(MuscleQueryOptions options = null)
    {
        // if null → create a new MuscleQueryOptions object
        options ??= new MuscleQueryOptions(); 
        
        return new SupabaseQueryBuilder()
            .StartWith("id", "code", "name", "latinName", "created_at", "updated_at")
            .AddIf(options.IncludePlaneMovement, "muscle_actions_planes:muscle_actions_planes_muscle_id_fkey(plane_id, Planes(name, description))")
            .AddIf(options.IncludeMuscleRegion, "muscle_regions:muscle_regions_muscle_id_fkey(region_id, BodyRegions!muscle_regions_region_id_fkey(name, latinName))")
            .AddIf(options.IncludeJointActions, "muscle_actions_joint:muscle_actions_muscle_id_fkey(joint_id, action_id, Joint(name, latinName), Actions(name, description))");
    }
    
    
    public string Build()
    {
        return string.Join(", ", _fields);
    }
}