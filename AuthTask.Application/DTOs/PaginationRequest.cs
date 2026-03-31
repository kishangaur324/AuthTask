namespace AuthTask.Application.DTOs
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; } = null;
        public int Skip => PageNumber > 0 ? (PageNumber - 1) * PageSize : 0;
    }
}
