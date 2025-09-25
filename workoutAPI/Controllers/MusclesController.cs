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
        [FromQuery] bool? includePlaneMovement = false,
        [FromQuery] bool? includeMuscleRegion = false,
        [FromQuery] bool? includeJointActions = false,
        [FromQuery] bool? includeMovementType = false
        )
    {
        string queryString = "id, code, name, latinName, created_at, updated_at";
        if (includePlaneMovement == true)
        {
            queryString += ", muscle_actions_planes:muscle_actions_planes_muscle_id_fkey(plane_id, Planes(name, description))";
        }
        if (includeJointActions == true)
        {
            queryString += ", muscle_actions_joint:muscle_actions_muscle_id_fkey(joint_id, action_id, Joint(name, latinName), Actions(name, description))";  
        }

        if (includeMuscleRegion == true)
        {
            queryString +=
                ", muscle_regions:muscle_regions_muscle_id_fkey(region_id, BodyRegions!muscle_regions_region_id_fkey(name, latinName))";
        }
      
        
        try
        {
            var response = await _supabase
                .From<MuscleTable>()
                .Select(queryString)
                .Get();
            
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