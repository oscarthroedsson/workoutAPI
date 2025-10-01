using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace workoutAPI.Models.Equipment;

[Table("Equipment")]
public class EquipmentDTO : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("code")]
    public string Code { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class Equipment
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}

public class EquipmentSmall
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}