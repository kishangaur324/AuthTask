namespace AuthTask.Application.DTOs
{
    public class PaginationResponse<T>
    {
        public int TotalCount { get; set; }
        public required List<T> Items { get; set; }
    }
}
