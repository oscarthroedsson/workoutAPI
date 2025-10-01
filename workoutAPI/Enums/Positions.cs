namespace workoutAPI.Enums;

public enum PositionType
{
    Standing,
    Seated,
    Lying,
    Kneeling,
    Hanging,
    Lunging,
    Squatting,
    Plank,
    BentOver,
    SideLying,
    Decline,
    Incline,
    Handstanding
}

public static class PositionTypeExtensions
{
    private static readonly Dictionary<string, PositionType> CodeToEnum = new()
    {
        { "standing", PositionType.Standing },
        { "seated", PositionType.Seated },
        { "lying", PositionType.Lying },
        { "kneeling", PositionType.Kneeling },
        { "hanging", PositionType.Hanging },
        { "lunging", PositionType.Lunging },
        { "squatting", PositionType.Squatting },
        { "plank", PositionType.Plank },
        { "bentOver", PositionType.BentOver },
        { "sideLying", PositionType.SideLying },
        { "decline", PositionType.Decline },
        { "incline", PositionType.Incline },
        { "handstanding", PositionType.Handstanding }
    };
    
    public static PositionType? FromCode(string code)
    {
        return CodeToEnum.TryGetValue(code, out var type) ? type : null;
    }
}