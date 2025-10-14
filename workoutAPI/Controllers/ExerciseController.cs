using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Supabase.Postgrest;
using workoutAPI.Headers;
using workoutAPI.Mappers;
using workoutAPI.Models;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Exercise;
using workoutAPI.Models.Pagination;
using workoutAPI.Models.Position;
using workoutAPI.Models.Requests;
using workoutAPI.Service;
using Client = Supabase.Client;
using workoutAPI.Utilities;
using Constants = Supabase.Postgrest.Constants;

namespace workoutAPI.Controllers;

[Route("api/exercise")]
public class ExerciseController : Controller
{
    private readonly Client _supabase;
    private readonly PointCalculatorService _pointCalculatorService;
    private readonly HeaderManager _headerManager;
    public ExerciseController(Client supabase, PointCalculatorService pointCalculatorService, HeaderManager headerManager)
    {
        _supabase = supabase;
        _pointCalculatorService = pointCalculatorService;
        _headerManager = headerManager;
    }
    
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok("api/Exercise is live");
    }

    [HttpGet("")]
    public async Task<IActionResult> Get(
        [FromQuery] GetExercisesRequest req
     )
    {
        req.Order ??= "asc";
        req.Sort ??= "name";
        
        var ordering = req.Order.ToLower() == "desc" ? Constants.Ordering.Descending : Constants.Ordering.Ascending;
       
        // Will get the IDs so we can filter the query
        var tasks = new[]
        {
            CodeToIdService.GetIdByCodeAsync<PlaneDTO>(_supabase, req.Plane),
            CodeToIdService.GetIdByCodeAsync<BodyRegionsDTO>(_supabase, req.BodyRegion),
            CodeToIdService.GetIdByCodeAsync<PositionDTO>(_supabase, req.Position),
            CodeToIdService.GetIdByCodeAsync<BodyMovementDTO>(_supabase, req.BodyMovement)
        };
        var results = await Task.WhenAll(tasks);
        var (planeID, bodyRegionID, positionID, bodyMovementID) = (results[0], results[1], results[2], results[3]);
        
        
        if (req.IncludeDetail)
        {
            req.IncludeInstruction = true;
            req.IncludeDescription = true;
            req.IncludePlane = true;
            req.IncludeBodyMovement = true;
        }
        
        var options = new ExerciseQueryOptions()
        {
            IncludeDetail = req.IncludeDetail,
            IncludeInstruction= req.IncludeInstruction,
            IncludeDescription = req.IncludeDescription,
            IncludePlane = req.IncludePlane,
            IncludeBodyMovement = req.IncludeBodyMovement
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
            .AddFilterIf(!string.IsNullOrEmpty(req.Plane), "plane_id", "eq", planeID)
            .AddFilterIf(!string.IsNullOrEmpty(req.BodyRegion), "primaryBodyRegion_id", "eq", bodyRegionID)
            .AddFilterIf(!string.IsNullOrEmpty(req.Position), "position_id", "eq", positionID)
            .AddFilterIf(!string.IsNullOrEmpty(req.BodyMovement), "bodyMovement_id", "eq", bodyMovementID);
        
        var fields = queryBuilder.Build();
        var filters = queryBuilder.BuildFilters();
        
        try
        {
            var response = await _supabase
                .From<ExerciseDTO>()
                .Select(fields)
                .ApplyFilters(filters)
                .Range(req.Offset, req.Offset + req.Number)
                .Order(req.Sort, ordering)
                .Get();
            
            var exercises = response.Models.Select(ExerciseMapper.MapFromTable);
            var pagination = Pagination.CreateMetadata(
                903,
                req.Offset,
                req.Number,
                exercises.Count()
            );
            
            decimal arrayPoints = _pointCalculatorService.CalculateListCost(exercises);
            _headerManager.IncrementHeader(HeaderKey.QuotaRequested, arrayPoints);
            
            
            return Ok(JSONResponse.Success(exercises, new{pagination}));
        }
        catch (Exception ex)
        {
            return BadRequest(JSONResponse.Error((ex.Message)));
        }
       
    }

    [HttpGet("{exerciseID}")]
    public async Task<IActionResult>  Get(
        string exerciseID,
        [FromQuery] GetExercisesRequest req
        )
    {
        
        if (req.IncludeDetail)
        {
            req.IncludeInstruction = true;
            req.IncludeDescription = true;
            req.IncludePlane = true;
            req.IncludeBodyMovement = true;
        }
        
        var options = new ExerciseQueryOptions()
        {
            IncludeDetail = req.IncludeDetail,
            IncludeInstruction= req.IncludeInstruction,
            IncludeDescription = req.IncludeDescription,
            IncludePlane = req.IncludePlane,
            IncludeBodyMovement = req.IncludeBodyMovement
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
        [FromQuery] bool includeBodyMovement = false,
        [FromQuery] int offset = 0,
        [FromQuery] int number = 50,
        [FromQuery] string sort = "name",
        [FromQuery] string order = "asc"
        )
    {
        // This is required
        if(query.IsNullOrEmpty()) return BadRequest("Query parameter is required.");
        var ordering = order.ToLower() == "desc" ? Constants.Ordering.Descending : Constants.Ordering.Ascending;
        
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
            return BadRequest(JSONResponse.Error((ex.Message)));
        }
    }
}