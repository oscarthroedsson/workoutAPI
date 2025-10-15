using workoutAPI.Configuration;
using workoutAPI.Service.Definitions;

namespace workoutAPI.Service;

public class PointCalculatorService
{
    private static readonly Dictionary<string, decimal> _costsLookup;
    
    static PointCalculatorService()
    {
        // Build object of queryParameter = points so we can count points easy
        _costsLookup = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        
        var type = typeof(PointCosts);
        var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        Console.WriteLine("=== LOADING POINT COSTS ===");
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(decimal))
            {
                var fieldName = field.Name;
                var value = (decimal)field.GetValue(null)!;
                _costsLookup[fieldName] = value;
                Console.WriteLine($"🔺 Loaded: {fieldName} = {value}");
            }
        }
        Console.WriteLine($"Total loaded: {_costsLookup.Count}");
        Console.WriteLine("===========================");
    }
    
    
    public decimal CalculateRequestCost(HttpContext context)
    {
        decimal points = PointCosts.BaseRequest;
        var query = context.Request.Query;
        
        foreach (var key in query.Keys)
        {
            points += CalculateParameterCost(key, query[key].ToString());
        }
        
        return points;
    }
    
     public decimal CalculateListCost(IEnumerable<object> items)
    {
        return items.Count() * PointCosts.ListItemReturned;
    }
    private decimal CalculateParameterCost(string paramName, string paramValue)
    {
        if (!_costsLookup.TryGetValue(paramName.ToLower(), out var cost) || cost == 0) return 0;
   
        // Check if queryParams exist and if the value match the enums
        if (
            ValidParameterValues.Values.TryGetValue(paramName, out var validValues) && 
            validValues.Contains(paramValue)
            )
        {
            return  cost; 
        }

        return 0; 
    }

   
}