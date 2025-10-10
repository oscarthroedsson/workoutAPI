namespace workoutAPI.Configuration;

public static class Tiers  // ← Lägg till 'static' här!
{
    public const string FREE = "FREE";
    public const string HOBBY = "HOBBY";
    public const string STARTUP = "STARTUP";
    public const string BUSINESS = "BUSINESS";
    
    // Points Limits (monthly quota)
    public const int FreeLimit = 50;
    public const int HobbyLimit = 1500;
    public const int StartUpLimit = 4500;
    public const int BusniessLimit = 10000;
    
    // Extra cost per point exceed limit
    public const decimal HobbyExtraCostPerPoint = 0.006m;
    public const decimal StartUpExtraCostPerPoint = 0.005m;
    public const decimal BusniessExtraCostPerPoint = 0.004m;
    
    // Rate limits (requests per second)
    public const int FreeMaxReqPerSecond = 2;
    public const int HobbyMaxReqPerSecond = 5;
    public const int StartUpMaxReqPerSecond = 10;
    public const int BusniessMaxReqPerSecond = 20;
}