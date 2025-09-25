
public class SupabaseQueryBuilder
{
    private List<string> _fields = new List<string>();
    
    public SupabaseQueryBuilder StartWith(params string[] baseFields)
    {
        _fields.AddRange(baseFields);
        return this; // make it possible to chain functions
    }
    
    public SupabaseQueryBuilder AddIf(bool include, string relationQuery)
    {
        if (include) _fields.Add(relationQuery);
        return this; // make it possible to chain functions
    }

    public SupabaseQueryBuilder Add(string baseFields)
    {
        _fields.Add(baseFields);
        return this;    // make it possible to chain functions
    }
    
    
    
    public string Build()
    {
        return string.Join(", ", _fields);
    }
}