namespace workoutAPI.Models.Equipment;

public class Equipment
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}

public class EquipmentSmall
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }

}