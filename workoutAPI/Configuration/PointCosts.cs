namespace workoutAPI.Configuration;






public class PointCosts
{
    // Base costs
    public const decimal BaseRequest = 1.0m;
    public const decimal query = 0.25m;
    public const decimal ListItemReturned = 0.01m; 
    
    // Params for Exercise-endpoint
    public const decimal IncludeDetails = 1.0m;
    public const decimal IncludeInstructions = 0.25m;
    public const decimal IncludeDescription = 0.25m;
    public const decimal IncludePlane = 0.25m;
    public const decimal IncludeBodyMovement = 0.25m;
    
    // filter params 
    public const decimal BodyMovement = 0.25m;
    public const decimal Plane = 0.25m;
    public const decimal BodyRegion = 0.25m;
    public const decimal Position = 0.25m;
    
    // Params for Muscle-endpoint
    public const decimal IncludePlaneMovement = 0.25m;
    public const decimal IncludeJointActions = 0.25m;
    public const decimal IncludeMuscleRegion = 0.25m;
    public const decimal IncludeMuscles = 0.25m;

}