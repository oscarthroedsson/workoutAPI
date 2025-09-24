using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("Muscles")]
public class MuscleTable : BaseModel

{   
    // Primary muscle data from muscle table
    [PrimaryKey("id")]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("latinName")]
    public string? LatinName { get; set; }

    [Column("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
    
    
    // Relationalships to mucles in MuscleTable
    public List<MuscleActionPlane>? muscle_actions_planes { get; set; }
    public List<MuscleActionJoint>? muscle_actions_joint { get; set; }
}

public class MuscleActionPlane
{
    [Column("plane_id")]
    public int PlaneId { get; set; }

    public Plane? Planes { get; set; }
}

public class Plane
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }
}

public class MuscleActionJoint
{
    [Column("joint_id")]
    public int JointId { get; set; }

    [Column("action_id")]
    public int ActionId { get; set; }

    public Joint? Joint { get; set; }
    public ActionDetail? Actions { get; set; }
}

public class Joint
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("latinName")]
    public string? LatinName { get; set; }
}

public class ActionDetail
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }
}