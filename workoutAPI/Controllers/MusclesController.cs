using Microsoft.AspNetCore.Mvc;
using Supabase;
using workoutAPI.Models.Muscle;
using workoutAPI.Services;


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
        var options = new MuscleQueryOptions
        {
            IncludePlaneMovement = includePlaneMovement,
            IncludeMuscleRegion = includeMuscleRegion,
            IncludeJointActions = includeJointActions
        };
        var query = QueryHelpers.DefaultQueryMuscle(options).Build();
        
        try
        {
            var response = await _supabase
                .From<MuscleTable>()
                .Select(query)
                .Get();
            
            var muscles = response.Models.Select(MuscleMapper.MapFromTable).ToList();
            
            return Ok(muscles);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching muscles: {ex.Message}");
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
        var query = QueryHelpers.DefaultQueryMuscle(options).Build();

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
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while fetching the muscle");
        }
        
    }

    
}