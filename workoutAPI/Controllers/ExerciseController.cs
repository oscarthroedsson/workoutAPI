using Microsoft.AspNetCore.Mvc;
using Supabase;
using workoutAPI.Mappers;
using workoutAPI.Models.Exercise;
using workoutAPI.Models.Position;

namespace workoutAPI.Controllers;

[Route("api/exercise")]
public class ExerciseController : Controller
{
    private readonly Client _supabase;

    public ExerciseController(Client supabase)
    {
        _supabase = supabase;
    }
    
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok("api/Exercise is live");
    }

    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var response = await _supabase.From<ExerciseDTO>()
                .Select(@" 
                    id,
                    name,
                    instructions,
                    description,
                    Equipment:equipment_id(id, code, name),
                    BodyRegions:primaryBodyRegion_id(id,code, name, latinName),
                    Planes:plane_id(id,code, name, description),
                    BodyMovements:bodyMovement_id(id, code, name, description),
                    Positions:position_id(id, code, name, description)
                ")
                .Limit(5)
                .Get();

            var exercises = response.Models.Select(ExerciseMapper.MapFromTable);
            return Ok(exercises);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
       
    }
    
    
    
    
    
}