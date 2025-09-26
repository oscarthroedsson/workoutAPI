using Microsoft.AspNetCore.Http.HttpResults;
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

    [HttpGet("{bodyRegionID}")]
    public async Task<IActionResult> Get(int bodyRegionID,
        [FromQuery] bool includeMuscles = false)
    {
        try
        {
            var options = new BodyRegionQueryOptions
            {
                IncludeMuscles = includeMuscles
            };

            var query = QueryHelpers.DefaultQueryBodyRegion(options).Build();
            var response = await _supabase.From<BodyRegionsDTO>().Select(query).Where(x => x.Id == bodyRegionID).Get();
            
            var bodyRegions = response.Models.Select(BodyRegionMapper.MapFromTable).ToList();
            return Ok(bodyRegions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while fetching the muscle region");
        }



    }


}