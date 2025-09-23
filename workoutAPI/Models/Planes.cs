namespace workoutAPI.Models;

public class Planes
{
    public Guid? Id { get; set; }   // optional
    public string code { get; set; }
    public string name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}