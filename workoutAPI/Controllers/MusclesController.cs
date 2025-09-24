using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Supabase;
using workoutAPI.Models;
using workoutAPI.Models.Muscle;


namespace workoutAPI.Controllers;

[Route("api/muscles")]
public class MuscleController : Controller
{
    private readonly Client _supabase;

    public MuscleController(Client supabase)
    {
        _supabase = supabase;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? includePlane = false,
        [FromQuery] bool? includeActions = false,
        [FromQuery] bool? includeJoints = false,
        [FromQuery] bool? includeMovementType = false
        )
    {
        string queryString = "";
        if (includePlane == true) queryString += "muscle_actions_planes!muscle_id(plane_id, Planes(name, description))";
        
        try
        {
            var response = await _supabase
                .From<MuscleTable>()
                .Select(@"
                    id, code, name, latinName, created_at, updated_at,
                    muscle_actions_planes:muscle_actions_planes_muscle_id_fkey(
                        plane_id,
                        Planes(name, description)
                        ),
                     muscle_actions_joint:muscle_actions_muscle_id_fkey(
                        joint_id,
                        action_id,
                        Joint(name, latinName),
                        Actions(name, description)
        )(
                        joint_id,
                        action_id,
                        Joint(name, latinName),
                        Actions(name, description)
                        )
    ")
                .Get();

            var apa = response.Models;
            var muscles = response.Models.Select(MuscleMapper.MapFromTable).ToList();

           
            
            return Ok(muscles);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching muscles: {ex.Message}");

            // Returnera 500 Internal Server Error med meddelande
            return StatusCode(500, new { error = "Failed to fetch muscles", details = ex.Message });
        }
       
    }
}