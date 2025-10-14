using workoutAPI.Service;

namespace workoutAPI.Middlewear;

public class PointsCalculationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly PointCalculatorService _calculator;
    
    public PointsCalculationMiddleware(RequestDelegate next, PointCalculatorService calculator)
    {
        _next = next;
        _calculator = calculator;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        decimal points = _calculator.CalculateRequestCost(context);
        context.Items["RequestPoints"] = points;
        
        await _next(context);
    }
}