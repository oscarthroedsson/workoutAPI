namespace workoutAPI;

public class ExerciseMiddleWear
{
    private readonly RequestDelegate _next;
    
    public ExerciseMiddleWear(RequestDelegate next)  // Matchar klassnamnet nu!
    {
        _next = next;
    }
    
    // Denna metod körs för varje request
    
    public async Task InvokeAsync(HttpContext context)
    {
        // validera apiKey (generell klass)
        // räkna poäng (En stor poängräknare eller en för varje end-point får se vad jag gör)
        // hanterar rate middle wear
        
        // är alt ok kör denna
        await _next(context); 
        
        // Kod EFTER endpoint (response processing)
        // har allt gott ok så kan vi ha en metod i rateLimitMiddlewear som är addUsage
    }
}