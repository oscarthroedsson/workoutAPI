using Microsoft.AspNetCore.Mvc;
using Supabase.Postgrest;
using workoutAPI.Models;
using workoutAPI.Models.ApiKey;
using Client = Supabase.Client;
using workoutAPI.Models.User;
using workoutAPI.Service;

namespace workoutAPI.Controllers;

[Route("api/auth/user")]
public class AuthController : Controller
{
  private readonly Client _supabase;

  public AuthController(Client supabase)
  {
    _supabase = supabase;
  }

  
  [HttpPost("create")]
  public async Task<IActionResult> POST([FromBody] CreateUser userPayload)
  {
    var test = userPayload;
    var response = await _supabase
      .From<UserDTO>()
      .Where(x => x.Email == userPayload.Email)
      .Get();
    
    bool userExists = response.Models.Count > 0;
    if(userExists)return Conflict("Email already exists");

    try
    {
      var userResponse = await _supabase.From<UserDTO>().Insert(new UserDTO
      {
        Name = userPayload.Name,
        Email = userPayload.Email,
        Tier = userPayload.Tier,
        Provider = userPayload.Provider,
        ProviderID = userPayload.ProviderID,
      });
      var newUser = userResponse.Models.FirstOrDefault();
      if (newUser == null) return BadRequest("User could not be created");
      
      string apiKey = ApiKeyService.GenerateApiKey();
      var newApi = await _supabase.From<ApiKeyDTO>().Insert(new ApiKeyDTO
      {
        UserID = newUser.Id,
        Key = apiKey,
        Name = newUser.Name,
        IsActive = true,
        ReqToday = 0,
        LastResetDate = DateTime.UtcNow,
      });
      if(newApi == null) return BadRequest("ApiKey could not be created");
      
      
      return Created($"/api/auth/user/{newUser.Id}", JSONResponse.Success(new
      {
        user = newUser,
        apiKey
      }));
    }
    catch (Exception e)
    {
      Console.WriteLine("🚨 ERROR | auth/user/create | POST");
      Console.WriteLine(e.Message);
      
      return BadRequest("Error creating user"); 
    }
   
    
    


  }
   
}