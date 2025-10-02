using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Supabase.Postgrest;
using workoutAPI.Mappers;
using workoutAPI.Models;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Exercise;
using workoutAPI.Models.Position;
using Client = Supabase.Client;
using workoutAPI.Utilities;



namespace workoutAPI.Controllers;



[Route("api/constants")]
public class ConstantController : Controller
{
    private readonly Client _supabase;

    public ConstantController(Client supabase)
    {
        _supabase = supabase;
    }

    [HttpGet("")]
    public async Task<IActionResult> Get(
        [FromQuery] bool includeAll,
        [FromQuery] bool includeActions,
        [FromQuery] bool includeBodyRegions,
        [FromQuery] bool includeDirections,
        [FromQuery] bool includeEquipments,
        [FromQuery] bool includeJoints,
        [FromQuery] bool includeBodyMovements,
        [FromQuery] bool includePlanes,
        [FromQuery] bool includePositions
        )
    {

        if (includeAll)
        {
            includeActions = true;
            includeBodyRegions = true;
            includeBodyRegions = true;
            includeDirections = true;
            includeEquipments = true;
            includeJoints = true;
            includeBodyMovements =  true;
            includePlanes =  true;
            includePositions = true;
        }
       
             var filters = new SupabaseQueryBuilder()
                .StartWith("*")
                .AddFilterIf(!includeActions, "table_name", "neq", "Actions")
                .AddFilterIf(!includeBodyRegions, "table_name", "neq", "BodyRegions")
                .AddFilterIf(!includeDirections, "table_name", "neq", "Directions")
                .AddFilterIf(!includeEquipments, "table_name", "neq", "Equipment")
                .AddFilterIf(!includeJoints, "table_name", "neq", "Joints")
                .AddFilterIf(!includeBodyMovements, "table_name", "neq", "BodyMovement")
                .AddFilterIf(!includePlanes, "table_name", "neq", "Planes")
                .AddFilterIf(!includePositions, "table_name", "neq", "Positions")
                .BuildFilters();  
        

      
        try
        {
            var allCodes = await _supabase
                .From<ConstantsDTO>()
                .Select("*")
                .ApplyFilters(filters)
                .Get();
            
            var constants = ConstantsMapper.MapFromTable(allCodes.Models.ToList());
            return Ok(constants);
            
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
    
    
    
}