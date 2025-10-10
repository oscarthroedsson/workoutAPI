namespace workoutAPI.Configuration;

public class PointCosts
{
    // Base costs
    public const int BaseRequest = 1;
    public const double query = 0.25;
    public const double listItemReturned = 0.01; 
    
    // Params for Exercise-endpoint
    public const int IncludeDetails = 1;
    public const double includeInstructions = 0.25;
    public const double includeDescription = 0.25;
    public const double includePlane = 0.25;
    public const double includeBodyMovement = 0.25;
    
    // filter params 
    public const double bodyMovement = 0.25;
    public const double plane = 0.25;
    public const double bodyRegion = 0.25;
    public const double position = 0.25;
    
    // Params for Muscle-endpoint
    public const double includePlaneMovement = 0.1;
    public const double includeJointActions = 0.1;
    public const double includeMuscleRegion = 0.1;
    public const double includeMuscles = 0.1;

}