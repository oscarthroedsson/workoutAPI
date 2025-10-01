namespace workoutAPI.Enums;

public enum BodyMovementType
{
    Push,
    Pull,
    Rotate,
    Stabilize
}

public static class BodyMovementTypeExtensions
{
    private static readonly Dictionary<string, BodyMovementType> CodeToEnum = new()
    {
        { "push", BodyMovementType.Push },
        { "pull", BodyMovementType.Pull },
        { "rotate", BodyMovementType.Rotate },
        { "stabilize", BodyMovementType.Stabilize }
    };
    
    public static BodyMovementType? FromCode(string code)
    {
        return CodeToEnum.TryGetValue(code, out var type) ? type : null;
    }
}