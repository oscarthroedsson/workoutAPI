namespace workoutAPI.Enums;

public enum BodyRegionType
{
    Abdominals,
    Abductors,
    Adductors,
    Biceps,
    Calves,
    Chest,
    Forearms,
    Glutes,
    Hamstrings,
    Lats,
    Neck,
    LowerBack,
    MiddleBack,
    Quadriceps,
    Traps,
    Triceps,
    Shoulders,
    FrontShoulder,
    SideShoulder,
    BackShoulder
}

public static class BodyRegionTypeExtensions
{
    private static readonly Dictionary<string, BodyRegionType> CodeToEnum = new()
    {
        { "abdominals", BodyRegionType.Abdominals },
        { "abductors", BodyRegionType.Abductors },
        { "adductors", BodyRegionType.Adductors },
        { "biceps", BodyRegionType.Biceps },
        { "calves", BodyRegionType.Calves },
        { "chest", BodyRegionType.Chest },
        { "forearms", BodyRegionType.Forearms },
        { "glutes", BodyRegionType.Glutes },
        { "hamstrings", BodyRegionType.Hamstrings },
        { "lats", BodyRegionType.Lats },
        { "neck", BodyRegionType.Neck },
        { "lowerBack", BodyRegionType.LowerBack },
        { "middleBack", BodyRegionType.MiddleBack },
        { "quadriceps", BodyRegionType.Quadriceps },
        { "traps", BodyRegionType.Traps },
        { "triceps", BodyRegionType.Triceps },
        { "shoulders", BodyRegionType.Shoulders },
        { "frontShoulder", BodyRegionType.FrontShoulder },
        { "sideShoulder", BodyRegionType.SideShoulder },
        { "backShoulder", BodyRegionType.BackShoulder }
    };
    
    public static BodyRegionType? FromCode(string code)
    {
        return CodeToEnum.TryGetValue(code, out var type) ? type : null;
    }
}