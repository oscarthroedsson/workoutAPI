using Microsoft.AspNetCore.Mvc;
using Supabase;
using workoutAPI.Mappers;
using workoutAPI.Models.BodyRegions;

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
    public async Task<IActionResult> GetAll()
    {

        try
        {
            var response = await _supabase.From<BodyRegionsDTO>().Select(@"id, code, name, latinName,
            muscle_regions:muscle_regions_region_id_fkey(
                id, muscle_id,
                muscles:muscle_regions_muscle_id_fkey(
                    id, code, name, latinName
                    )
                )
            ").Get();

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