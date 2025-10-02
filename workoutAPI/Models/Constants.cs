using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("constants")]
public class ConstantsDTO : BaseModel
{
    [Column("table_name")]
    public string TableName { get; set; }

    [Column("code")]
    public string Code { get; set; }
}

public class ConstantsModel : Dictionary<string, ConstantGroup>
{
}

public class ConstantGroup : Dictionary<string, string>
{
    public List<string> Arr { get; set; } = new List<string>();
}

