namespace workoutAPI.Models.Position;

public class PositionModel
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } 
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}

public class PositionSmall
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } 
}