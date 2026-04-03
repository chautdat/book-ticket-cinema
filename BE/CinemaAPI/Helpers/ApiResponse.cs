namespace cinema.Helpers
{
    public class ApiResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static ApiResponse Success(string message = "", object? data = null) =>
            new ApiResponse { Status = true, Message = message, Data = data };

        public static ApiResponse Fail(string message = "", object? data = null) =>
            new ApiResponse { Status = false, Message = message, Data = data };
    }
}
