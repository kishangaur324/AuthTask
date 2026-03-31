namespace AuthTask.Application.DTOs
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string? Error { get; set; }
    }
}
