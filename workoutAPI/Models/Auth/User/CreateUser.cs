using System.Text.Json.Serialization;

namespace workoutAPI.Models.User;

public class CreateUser
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("provider")]
    public string Provider { get; set; }
    [JsonPropertyName("providerID")]
    public string ProviderID { get; set; }
    [JsonPropertyName("tier")]
    public string Tier { get; set; }
}