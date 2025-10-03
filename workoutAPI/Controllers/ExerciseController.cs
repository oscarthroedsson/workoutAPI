using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Supabase.Postgrest;
using workoutAPI.Mappers;
using workoutAPI.Models;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Exercise;
using workoutAPI.Models.Pagination;
using workoutAPI.Models.Position;
using Client = Supabase.Client;
using workoutAPI.Utilities;
using Constants = Supabase.Postgrest.Constants;

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
        [FromQuery] string bodyMovement = "",
        [FromQuery] int offset = 0,
        [FromQuery] int number = 50,
        [FromQuery] string sort = "name",
        [FromQuery] string order = "asc"
     )
    {
        var ordering = order.ToLower() == "desc" ? Constants.Ordering.Descending : Constants.Ordering.Ascending;
       
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
                .Range(offset, offset + number)
                .Order(sort, ordering)
                .Get();
            
            var exercises = response.Models.Select(ExerciseMapper.MapFromTable);
            
            var pagination = Pagination.CreateMetadata(
                903,
                offset,
                number,
                exercises.Count()
            );

           
            return Ok(JSONResponse.Success(exercises, new{pagination}));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
       
    }

    [HttpGet("{exerciseID}")]
    public async Task<IActionResult>  Get(
        string exerciseID,
        [FromQuery] bool includeDetail = false,
        [FromQuery] bool includeInstruction = false,
        [FromQuery] bool includeDescription = false,
        [FromQuery] bool includePlane = false,
        [FromQuery] bool includeBodyMovement = false
        )
    {
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
            .AddIf(options.IncludeBodyMovement, "BodyMovements:bodyMovement_id(id, code, name, description)");
        
        var query = queryBuilder.Build();

        try
        {
            var response = await _supabase
                .From<ExerciseDTO>()
                .Select(query)
                .Where(x => x.Id == exerciseID)
                .Get();
            
            
            var exercises = response.Models.Select(ExerciseMapper.MapFromTable);
            return Ok(exercises);
            
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery, Required] string query,
        [FromQuery, ] bool includeDetail = false,
        [FromQuery] bool includeInstruction = false,
        [FromQuery] bool includeDescription = false,
        [FromQuery] bool includePlane = false,
        [FromQuery] bool includeBodyMovement = false
        )
    {
        // This is required
        if(query.IsNullOrEmpty()) return BadRequest("Query parameter is required.");
        
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
        try
        {
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
                .AddIf(options.IncludeBodyMovement, "BodyMovements:bodyMovement_id(id, code, name, description)");
        
            var fields = queryBuilder.Build();
            var response = await _supabase
                .From<ExerciseDTO>()
                .Select(fields)
                .Filter(x => x.SearchVector, Constants.Operator.FTS, new FullTextSearchConfig(query, "english"))
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