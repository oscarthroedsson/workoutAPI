using System.Security.Cryptography;

namespace workoutAPI.Service;

public class ApiKeyService
{

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
}