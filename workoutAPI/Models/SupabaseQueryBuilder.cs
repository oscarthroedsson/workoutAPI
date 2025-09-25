namespace workoutAPI.Models;

public class SupabaseQueryBuilder
{
    public bool Include { get; set; }
    public string Query { get; set; }        // Istället för QueryParam
    public string Value { get; set; }
}