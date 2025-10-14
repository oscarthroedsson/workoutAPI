using workoutAPI.Models.ApiKey;
using workoutAPI.Models.BodyRegions;
using workoutAPI.Models.Position;

namespace workoutAPI.Models.Cache;

public class StaticDataDTO
{
    public List<PlaneDTO> Planes { get; set; } = new();
    public List<BodyRegionsDTO> BodyRegions { get; set; } = new();
    public List<PositionDTO> Positions { get; set; } = new();
    public List<BodyMovementDTO> BodyMovements { get; set; } = new();
    
    public List<ApiKeyDTO> ApiKeys { get; set; } = new();
    
    public int? GetIdByCode<T>(string? code) where T : class
    {
        if (string.IsNullOrEmpty(code)) return null;
        
        var collection = typeof(T).Name switch
        {
            nameof(PlaneDTO) => Planes as IEnumerable<object>,
            nameof(BodyRegionsDTO) => BodyRegions as IEnumerable<object>,
            nameof(PositionDTO) => Positions as IEnumerable<object>,
            nameof(BodyMovementDTO) => BodyMovements as IEnumerable<object>,
            _ => throw new ArgumentException($"Unsupported type: {typeof(T).Name}")
        };
        
        // Use reflection to get Code and Id properties
        var item = collection?.FirstOrDefault(obj =>
        {
            var codeProperty = obj.GetType().GetProperty("Code");
            var codeValue = codeProperty?.GetValue(obj) as string;
            return codeValue?.Equals(code, StringComparison.OrdinalIgnoreCase) ?? false;
        });
        
        if (item == null) return null;
        
        var idProperty = item.GetType().GetProperty("Id");
        return idProperty?.GetValue(item) as int?;
    }
    public (int? planeId, int? bodyRegionId, int? positionId, int? bodyMovementId) 
        GetAllIds(string? planeCode, string? bodyRegionCode, string? positionCode, string? bodyMovementCode)
    {
        return (
            GetIdByCode<PlaneDTO>(planeCode),
            GetIdByCode<BodyRegionsDTO>(bodyRegionCode),
            GetIdByCode<PositionDTO>(positionCode),
            GetIdByCode<BodyMovementDTO>(bodyMovementCode)
        );
    }
    public ApiKeyDTO? GetApiKey(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) return null;

        return ApiKeys.FirstOrDefault(k =>
            k.Key.Equals(apiKey, StringComparison.OrdinalIgnoreCase));
    }
}