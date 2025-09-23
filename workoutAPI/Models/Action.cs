namespace workoutAPI.Models;

public class Action
{
    public Guid? Id { get; set; }   // optional
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }   // optional
    public DateTimeOffset? CreatedAt { get; set; }   // matches timestamptz
    public DateTimeOffset? UpdatedAt { get; set; }   // matches timestamptz
}

