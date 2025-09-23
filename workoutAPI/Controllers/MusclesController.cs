using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
namespace workoutAPI.Controllers;



[Route("api/muscles")]
public class MuscleController : Controller
{
    [HttpGet("")]
    public IActionResult GetAll()
    {
        
        // Importera min json
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data","Muscles", "muscles.json");
        string contents = System.IO.File.ReadAllText(filePath);
        
        // Göra om obj i arrayen till C# obj
        // Kunna retunera en array/lista wtf ever
        
        
        
        return View();
    }
}