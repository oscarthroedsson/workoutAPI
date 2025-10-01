namespace workoutAPI.Enums;

public enum ActionType
{
    Flexion,
    Extension,
    Abduction,
    Adduction,
    InternalRotation,
    ExternalRotation,
    HorizontalAbduction,
    HorizontalAdduction,
    Dorsiflexion,
    Plantarflexion,
    Inversion,
    Eversion,
    LateralFlexion,
    Rotation,
    PosteriorTilt,
    AnteriorTilt,
    DecreaseLordosis,
    IncreaseLordosis,
    Stabilization,
    Supination,
    Pronation,
    Protraction,
    Depression,
    Elevation,
    Retraction
}
public static class ActionTypeExtensions
{
    private static readonly Dictionary<string, ActionType> CodeToEnum = new()
    {
        { "flexion", ActionType.Flexion },
        { "extension", ActionType.Extension },
        { "abduction", ActionType.Abduction },
        { "adduction", ActionType.Adduction },
        { "internalRotation", ActionType.InternalRotation },
        { "externalRotation", ActionType.ExternalRotation },
        { "horizontalAbduction", ActionType.HorizontalAbduction },
        { "horizontalAdduction", ActionType.HorizontalAdduction },
        { "dorsiflexion", ActionType.Dorsiflexion },
        { "plantarflexion", ActionType.Plantarflexion },
        { "inversion", ActionType.Inversion },
        { "eversion", ActionType.Eversion },
        { "lateralFlexion", ActionType.LateralFlexion },
        { "rotation", ActionType.Rotation },
        { "posteriorTilt", ActionType.PosteriorTilt },
        { "anteriorTilt", ActionType.AnteriorTilt },
        { "decreaseLordosis", ActionType.DecreaseLordosis },
        { "increaseLordosis", ActionType.IncreaseLordosis },
        { "stabilization", ActionType.Stabilization },
        { "supination", ActionType.Supination },
        { "pronation", ActionType.Pronation },
        { "protraction", ActionType.Protraction },
        { "depression", ActionType.Depression },
        { "elevation", ActionType.Elevation },
        { "retraction", ActionType.Retraction }
    };
    
    public static ActionType? FromCode(string code)
    {
        return CodeToEnum.TryGetValue(code, out var type) ? type : null;
    }
}