using workoutAPI.Configuration;
namespace workoutAPI.Middlewear;

public class PointsCalculationMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Dictionary<string, int> _costsLookup;
    
    // Will run one time when constructor loads
    static PointsCalculationMiddleware()
    {
        _costsLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        
        var type = typeof(PointCosts);
        var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(int))
            {
                var fieldName = field.Name;
                var value = (int)field.GetValue(null)!;
                _costsLookup[fieldName] = value;
            }
        }
    }
    public PointsCalculationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        int points = CalculatePoints(context);
        context.Items["RequestPoints"] = points;
        await _next(context);
    }

    private int CalculatePoints(HttpContext context)
    {
        int points = PointCosts.BaseRequest;
        var query = context.Request.Query;
        

        foreach (var key in query.Keys) 
        {
            if (_costsLookup.TryGetValue(key, out var cost))
            {
                points += cost;
            }
        }

      

        return points;
    }

   

}