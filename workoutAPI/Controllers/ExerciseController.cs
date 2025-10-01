using Microsoft.AspNetCore.Mvc;
using workoutAPI.Mappers;
using workoutAPI.Models;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Exercise;
using workoutAPI.Models.Position;
using Client = Supabase.Client;
using workoutAPI.Utilities;


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
    public async Task<IActionResult> Get(
        [FromQuery] bool includeDetail = false,
        [FromQuery] bool includeInstruction = false,
        [FromQuery] bool includeDescription = false,
        [FromQuery] bool includePlane = false,
        [FromQuery] bool includeBodyMovement = false,
        [FromQuery] string bodyRegion = "",
        [FromQuery] string position = "",
        [FromQuery] string plane = "",
        [FromQuery] string bodyMovement = ""
     )
    {
        // Will get the IDs so we can filter the query
        var tasks = new[]
        {
            CodeToIdService.GetIdByCodeAsync<PlaneDTO>(_supabase, plane),
            CodeToIdService.GetIdByCodeAsync<BodyRegionsDTO>(_supabase, bodyRegion),
            CodeToIdService.GetIdByCodeAsync<PositionDTO>(_supabase, position),
            CodeToIdService.GetIdByCodeAsync<BodyMovementDTO>(_supabase, bodyMovement)
        };
        var results = await Task.WhenAll(tasks);
        var (planeID, bodyRegionID, positionID, bodyMovementID) = (results[0], results[1], results[2], results[3]);

        
        if (includeDetail)
        {
            includeInstruction = true;
            includeDescription = true;
            includePlane = true;
            includeBodyMovement = true;
        }
        
        var options = new ExerciseQueryOptions()
        {
            IncludeDetail = includeDetail,
            IncludeInstruction= includeInstruction,
            IncludeDescription = includeDescription,
            IncludePlane = includePlane,
            IncludeBodyMovement = includeBodyMovement
        };

        var queryBuilder = new SupabaseQueryBuilder()
            .StartWith(@"
        id,
        name,
        Equipment:equipment_id(id, code, name),
        BodyRegions:primaryBodyRegion_id(id, code, name, latinName),
        Positions:position_id(id, code, name, description)
    ")
            .AddIf(options.IncludeInstruction, "instructions")
            .AddIf(options.IncludeDescription, "description")
            .AddIf(options.IncludePlane, "Planes:plane_id(id,code, name, description)")
            .AddIf(options.IncludeBodyMovement, "BodyMovements:bodyMovement_id(id, code, name, description)")
            .AddFilterIf(!string.IsNullOrEmpty(plane), "plane_id", "eq", planeID)
            .AddFilterIf(!string.IsNullOrEmpty(bodyRegion), "primaryBodyRegion_id", "eq", bodyRegionID)
            .AddFilterIf(!string.IsNullOrEmpty(position), "position_id", "eq", positionID)
            .AddFilterIf(!string.IsNullOrEmpty(bodyMovement), "bodyMovement_id", "eq", bodyMovementID);
        
        var fields = queryBuilder.Build();
        var filters = queryBuilder.BuildFilters();
        
        try
        {
            
            var response = await _supabase
                .From<ExerciseDTO>()
                .Select(fields)
                .ApplyFilters(filters)
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