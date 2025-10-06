namespace workoutAPI.Models.User;

public class CreateUser
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Provider { get; set; }
    public string ProviderID { get; set; }
    public string Tier { get; set; }
}