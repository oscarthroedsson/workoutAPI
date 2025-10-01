namespace workoutAPI.Services;

public static class EnumMapper
{
   
        public static T? FromCode<T>(string code) where T : struct, Enum
        {
            // Get the corresponding Extensions class for this enum type
            var extensionsType = Type.GetType($"{typeof(T).FullName}Extensions");
        
            if (extensionsType == null)
                return null;
        
            // Call the FromCode method on that Extensions class
            var method = extensionsType.GetMethod("FromCode");
            var result = method?.Invoke(null, new object[] { code });
        
            return result as T?;
        }
    
}