using Supabase;
namespace workoutAPI.Extensions;

public static class SupabaseServiceExtensions
{
    public static async Task<WebApplicationBuilder> AddSupabaseAsync(this WebApplicationBuilder builder)
    {
        var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
        var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");

        var client = new Client(url, key, new SupabaseOptions { AutoConnectRealtime = true });
        await client.InitializeAsync();

        builder.Services.AddSingleton(client);

        return builder;
    }
}