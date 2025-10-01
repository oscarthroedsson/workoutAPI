
using Microsoft.AspNetCore.Mvc.Filters;
using Supabase.Postgrest;
using workoutAPI.Enums;


public class SupabaseQueryBuilder
{
    private List<string> _fields = new List<string>();
    private List<FilterItem> _filters = new List<FilterItem>();
    
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
    
    public SupabaseQueryBuilder AddFilterIf(bool condition, string field, string op, string value)
    {
        if (condition && !string.IsNullOrWhiteSpace(value))
        {
            _filters.Add(new FilterItem 
            { 
                Field = field, 
                Operator = op, 
                Value = value 
            });
        }
        return this;
    }

    public SupabaseQueryBuilder AddFilter(string field, string op, string value)
    {
        return AddFilterIf(true, field, op, value);
    }



    public class FilterItem
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    
    public string Build()
    {
        return string.Join(", ", _fields);
    }
    
    public List<FilterItem> BuildFilters()
    {
        return _filters;
    }
    
    
    
    
    
}