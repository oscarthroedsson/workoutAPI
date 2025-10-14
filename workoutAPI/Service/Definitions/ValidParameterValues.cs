using workoutAPI.Enums;

namespace workoutAPI.Service.Definitions;
/*
    The central control that holds the values to all parameters with enums. 
*/
using System;
using System.Collections.Generic;

public static class ValidParameterValues
{

    public static readonly Dictionary<string, HashSet<string>> Values = new(StringComparer.OrdinalIgnoreCase)
    {
        { "IncludeDetails", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },
        { "IncludeInstructions", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },
        { "IncludeDescription", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },
        { "IncludePlane", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },
        { "IncludeBodyMovement", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },
        { "IncludePlaneMovement", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },
        { "IncludeJointActions", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },
        { "IncludeMuscleRegion", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },
        { "IncludeMuscles", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true" } },

        // Filter-parametrar → alla giltiga enum-värden
        { "BodyRegion", new HashSet<string>(Enum.GetNames(typeof(BodyRegionType)), StringComparer.OrdinalIgnoreCase) },
        { "Plane", new HashSet<string>(Enum.GetNames(typeof(PlaneType)), StringComparer.OrdinalIgnoreCase) },
        { "Position", new HashSet<string>(Enum.GetNames(typeof(PositionType)), StringComparer.OrdinalIgnoreCase) },
        { "BodyMovement", new HashSet<string>(Enum.GetNames(typeof(BodyMovementType)), StringComparer.OrdinalIgnoreCase) },
    };
}