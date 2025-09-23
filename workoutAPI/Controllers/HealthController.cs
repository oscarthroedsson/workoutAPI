using Microsoft.AspNetCore.Mvc;
namespace workoutAPI.Controllers;



public class HealthController : Controller
{
    [HttpGet("/health")]
    public IActionResult Get(int age)
    {
        return Ok(new { 
                status = "success",
                message = "App is running", 
                age = age
            }
        );
    }
    
    
}