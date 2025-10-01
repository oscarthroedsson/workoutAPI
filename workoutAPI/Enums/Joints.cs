namespace workoutAPI.Enums;

public enum JointType
{
    Hip,
    Knee,
    Shoulder,
    ShoulderGirdle,
    Elbow,
    Ankle,
    Spine,
    Lumbopelvic,
    Wrist,
    Radioulnar
}

public static class JointTypeExtensions
{
    private static readonly Dictionary<string, JointType> CodeToEnum = new()
    {
        { "hip", JointType.Hip },
        { "knee", JointType.Knee },
        { "shoulder", JointType.Shoulder },
        { "shoulderGirdle", JointType.ShoulderGirdle },
        { "elbow", JointType.Elbow },
        { "ankle", JointType.Ankle },
        { "spine", JointType.Spine },
        { "lumbopelvic", JointType.Lumbopelvic },
        { "wrist", JointType.Wrist },
        { "radioulnar", JointType.Radioulnar }
    };
    
    public static JointType? FromCode(string code)
    {
        return CodeToEnum.TryGetValue(code, out var type) ? type : null;
    }
}