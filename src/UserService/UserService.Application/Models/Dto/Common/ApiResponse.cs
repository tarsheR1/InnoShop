namespace UserService.API.Dto.Common
{
    public class ApiResponse
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
    }
}
