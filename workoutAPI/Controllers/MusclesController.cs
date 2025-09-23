using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Supabase;
using workoutAPI.Models;

namespace workoutAPI.Controllers;



[Route("api/muscles")]
public class MuscleController : Controller
{
    private readonly Client _supabase;

    public MuscleController(Client supabase)
    {
        _supabase = supabase;
    }
    
    [HttpGet("/all")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await _supabase
                .From<MuscleTable>()
                .Get();

            var muscles = response.Models.Select(m => new MuscleBase
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                LatinName = m.LatinName,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList();

            return Ok(muscles);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching muscles: {ex.Message}");

            // Returnera 500 Internal Server Error med meddelande
            return StatusCode(500, new { error = "Failed to fetch muscles", details = ex.Message });
        }
       
    }
}