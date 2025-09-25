using Microsoft.AspNetCore.Mvc;
using Supabase;
using workoutAPI.Mappers;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Services;

namespace workoutAPI.Controllers;

[Route("api/bodyRegion")]
public class BodyRegionController : Controller
{
    private readonly Client _supabase;

    public BodyRegionController(Client supabase)
    {
        _supabase = supabase;
    }
    
    
    // health end-point
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok("api/bodyRegion is live");
    }
    
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeMuscles = false
        )
    {
        var options = new BodyRegionQueryOptions
        {
            IncludeMuscles = includeMuscles
        };
    
        var query = QueryHelpers.DefaultQueryBodyRegion(options).Build();
        try
        {
            var response = await _supabase.From<BodyRegionsDTO>().Select(query).Get();
            var bodyRegions = response.Models.Select(BodyRegionMapper.MapFromTable).ToList();
            return Ok(bodyRegions);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}