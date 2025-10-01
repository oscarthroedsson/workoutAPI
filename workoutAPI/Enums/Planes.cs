namespace workoutAPI.Enums;

public enum PlaneType
{
    Sagittal,
    Frontal,
    Transverse
}

public static class PlaneTypeExtensions
{
    private static readonly Dictionary<string, PlaneType> CodeToEnum = new()
    {
        { "sagittal", PlaneType.Sagittal },
        { "frontal", PlaneType.Frontal },
        { "transverse", PlaneType.Transverse }
    };
    
    public static PlaneType? FromCode(string code)
    {
        return CodeToEnum.TryGetValue(code, out var type) ? type : null;
    }
}