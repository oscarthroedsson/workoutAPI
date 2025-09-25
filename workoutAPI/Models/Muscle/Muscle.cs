namespace workoutAPI.Models.Muscle;

public class Muscle
{
    public Guid? Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string LatinName { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string MuscleRegion { get; set; }
    public List<PlanesSmall>? Planes { get; set; }
    public List<ActionSmall>? JointActions { get; set; }
}

public class MuscleSmall
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string LatinName { get; set; }
}