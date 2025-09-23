using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace workoutAPI.Models;


 // Muscle type for supabase table
[Table("Muscles")]
public class MuscleTable : BaseModel
{
    [Column("id")]
    public Guid Id { get; set; }

    [PrimaryKey("code")]
    [Column("code")]
    public string Code { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("latinName")]
    public string LatinName { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MuscleBase
{
    public Guid Id { get; set; }   // optional
    public string Code {get; set;}
    public string Name {get; set;}  
    public string LatinName {get; set;}
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}


public class Muscle : MuscleBase
{

    public List<Action>? Actions { get; set; }
    public List<string>? Planes { get; set; }
    public List<string>? Joints { get; set; }
    public List<string>? Directions { get; set; }
    
}


