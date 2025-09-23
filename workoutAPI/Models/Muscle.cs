namespace workoutAPI.Models;


public class Muscle
{
    public Guid? Id { get; set; }   // optional
    public string Code {get; set;}
    public string Name {get; set;}  
    public string LatinName {get; set;}
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    // optinal keys
    public List<Action>? Actions { get; set; }
    public List<string>? Planes { get; set; }
    public List<string>? Joints { get; set; }
    public List<string>? Directions { get; set; }
    
}

