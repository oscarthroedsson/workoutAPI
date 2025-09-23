namespace workoutAPI.Models;

public class Direction
{
    public Guid? Id { get; set; }   // optional
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }   // matches timestamptz
    public DateTimeOffset? UpdatedAt { get; set; }   // matches timestamptz
}