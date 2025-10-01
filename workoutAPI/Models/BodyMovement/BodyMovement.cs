namespace workoutAPI.Models;

public class BodyMovementModel
{
    public string Id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string createdAt { get; set; }
    public string updatedAt { get; set; }   
}

public class BodyMovementSmall
{
    public string Id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
    public string description { get; set; }
}

