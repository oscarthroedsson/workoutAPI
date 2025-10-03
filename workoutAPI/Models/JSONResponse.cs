namespace workoutAPI.Models
{
    public class JSONResponse
    {
        public string Status { get; set; }
        public object? Data { get; set; }
        public object? Meta { get; set; }
        public string? Message { get; set; }

        private JSONResponse() { }

        public static JSONResponse Success(object? data = null, object? meta = null)
        {
            return new JSONResponse
            {
                Status = "success",
                Data = data,
                Meta = meta
            };
        }

        public static JSONResponse Error(string message, string? code = null, object? details = null)
        {
            return new JSONResponse
            {
                Status = "error",
                Message = message,
                Meta = new { code, details }
            };
        }
    }
}