using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace workoutAPI.Models;


public class ActionBase
{
    public Guid? Id { get; set; }   // optional
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }   // optional
    public DateTimeOffset? CreatedAt { get; set; }   // matches timestamptz
    public DateTimeOffset? UpdatedAt { get; set; }   // matches timestamptz
}

public class ActionSmall
{
    public string Name { get; set; } = string.Empty;
    public string? LatinName { get; set; }
    public PlanesSmall Actions { get; set; }
}


// we need to build a extended version for the action end-point later on



