namespace workoutAPI.Routes;

public static class IndexRoutes
{
    public static void MapRoutes(this WebApplication app)
    {
        app.MapGet("/search", () => "Hello from Index!");
    }
}