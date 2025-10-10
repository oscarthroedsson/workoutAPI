using System.Security.Cryptography;
using Supabase;
using workoutAPI.Models.ApiKey;

namespace workoutAPI.Service;

public class ApiKeyService
{
    private readonly Client _supabase;
    public ApiKeyService(Client supabase)
    {
        _supabase = supabase;
    }
    public static string GenerateApiKey()
    {
        byte[] keyBytes = new byte[32];

        using (var randomNumber = RandomNumberGenerator.Create())
        {
            randomNumber.GetBytes(keyBytes);
        }
        
        string base64 = Convert.ToBase64String(keyBytes)
            .Replace('+', '-')  // Replace + with -
            .Replace('/', '_')  // Replace / with _
            .TrimEnd('=');

        return base64;
    }
    
    public async Task<bool> RevokeApiKey(string apiKeyId)
    {
        var response = await _supabase
            .From<ApiKeyDTO>()
            .Where(x => x.Id == apiKeyId)
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedAt, DateTime.UtcNow)
            .Update();
        
        return response.Models.Count > 0;
    }

    public async Task UpdateDailyUsage(ApiKeyDTO key, int points)
    {
        await _supabase
            .From<ApiKeyDTO>()
            .Where(x => x.Id == key.Id)
            .Set(x => x.ReqToday, key.ReqToday + points)
            .Set(x => x.UpdatedAt, DateTime.UtcNow)
            .Update();
    }
    
    

    
}