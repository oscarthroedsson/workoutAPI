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
    
    public async Task<List<ApiKeyDTO>> GetApiKeysForRecentDays()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-2);

        var response = await _supabase
            .From<ApiKeyDTO>()
            .Where(x => x.LastResetDate >= startDate)
            .Get();

        if (response.Models == null || !response.Models.Any())
            return new List<ApiKeyDTO>();

        return response.Models.ToList();
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
    
    
    public async Task<ApiKeyUserInfo?> GetUserInfoByApiKeyAsync(string apiKey)
    {
        try
        {
            var response = await _supabase
                .From<ApiKeyUserInfo>()
                .Where(x => x.ApiKey == apiKey && x.IsActive == true)
                .Single();

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⏰ {ex.Message}");
            return null;
        }
    }

    public async Task<List<ApiKeyUserInfo>> GetAllApiKeysWithUserInfoAsync()
    {
        try
        {
            var response = await _supabase
                .From<ApiKeyUserInfo>()
                .Get();

            return response.Models;
        }
        catch (Exception ex)
        {
          
            return new List<ApiKeyUserInfo>();
        }
    }
}