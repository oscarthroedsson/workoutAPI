using System;
using System.Threading.Tasks;
using Supabase;
using Supabase.Postgrest.Models;

namespace workoutAPI.Utilities
{
    public static class CodeToIdService
    {
        public static async Task<string?> GetIdByCodeAsync<TModel>(Client supabase, string code)
            where TModel : BaseModel, new()
        {
            try
            {
                var result = await supabase
                    .From<TModel>()
                    .Select("id")
                    .Filter("code", Supabase.Postgrest.Constants.Operator.Equals, code)
                    .Single();

                if (result == null)
                    return null;

                // Försök läsa "id" eller "Id"
                var idProperty = result.GetType().GetProperty("id") ??
                                 result.GetType().GetProperty("Id");

                if (idProperty != null)
                    return idProperty.GetValue(result)?.ToString();

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIdByCodeAsync: {ex.Message}");
                return null;
            }
        }
    }
}