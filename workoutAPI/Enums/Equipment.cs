namespace workoutAPI.Enums;

public enum EquipmentType
{
    Band,
    Barbell,
    BodyWeight,
    Cable,
    BosuBall,
    Dumbbell,
    Kettlebell,
    EzBarbell,
    MedicineBall,
    SlamBall,
    ResistanceBand,
    Rope,
    Sled,
    SmithMachine,
    StabilityBall,
    Tire,
    TrapBar
}

public static class EquipmentTypeExtensions
{
    private static readonly Dictionary<string, EquipmentType> CodeToEnum = new()
    {
        { "band", EquipmentType.Band },
        { "barbell", EquipmentType.Barbell },
        { "bodyWeight", EquipmentType.BodyWeight },
        { "cable", EquipmentType.Cable },
        { "bosuBall", EquipmentType.BosuBall },
        { "dumbbell", EquipmentType.Dumbbell },
        { "kettlebell", EquipmentType.Kettlebell },
        { "ezBarbell", EquipmentType.EzBarbell },
        { "medicineBall", EquipmentType.MedicineBall },
        { "slamBall", EquipmentType.SlamBall },
        { "resistanceBand", EquipmentType.ResistanceBand },
        { "rope", EquipmentType.Rope },
        { "sled", EquipmentType.Sled },
        { "smith_machine", EquipmentType.SmithMachine },
        { "stabilityBall", EquipmentType.StabilityBall },
        { "tire", EquipmentType.Tire },
        { "trapBar", EquipmentType.TrapBar }
    };
    
    public static EquipmentType? FromCode(string code)
    {
        return CodeToEnum.TryGetValue(code, out var type) ? type : null;
    }
}