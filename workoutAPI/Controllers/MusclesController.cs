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
        [FromQuery] bool includePlaneMovement = false,
        [FromQuery] bool includeMuscleRegion = false,
        [FromQuery] bool includeJointActions = false
        )
    {
        string queryString = "id, code, name, latinName, created_at, updated_at";
        if (includePlaneMovement)
        {
            queryString += ", muscle_actions_planes:muscle_actions_planes_muscle_id_fkey(plane_id, Planes(name, description))";
        }
        if (includeJointActions)
        {
            queryString += ", muscle_actions_joint:muscle_actions_muscle_id_fkey(joint_id, action_id, Joint(name, latinName), Actions(name, description))";  
        }

        if (includeMuscleRegion)
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

    [HttpGet("{muscleID}")]
    public async Task<IActionResult> Get(Guid muscleID, 
        [FromQuery] bool includePlaneMovement = false,
        [FromQuery] bool includeMuscleRegion = false,
        [FromQuery] bool includeJointActions = false)
    {
        if(muscleID == Guid.Empty) return BadRequest("Invalid muscle ID");
        
        var options = new MuscleQueryOptions
        {
            IncludePlaneMovement = includePlaneMovement,
            IncludeMuscleRegion = includeMuscleRegion,
            IncludeJointActions = includeJointActions
        };
        var query = SupabaseQueryBuilder.MuscleDefaultQuery(options).Build();

        try
        {
            var response = await _supabase
                .From<MuscleTable>()
                .Select(query)
                .Where(x => x.Id == muscleID)
                .Get();

            if (response.Models.Count < 1)
            {
                return NotFound($"Muscle with ID {muscleID} not found");
            }

            var mappedMuscle = MuscleMapper.MapFromTable(response.Models.First());
            return Ok(mappedMuscle);


        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while fetching the muscle");
        }
        
    }
}