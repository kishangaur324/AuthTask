namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Request payload for paginated list retrieval.
    /// </summary>
    public class PaginationRequest
    {
        /// <summary>
        /// Gets or sets the 1-based page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets page size.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Gets or sets optional search text.
        /// </summary>
        public string? Search { get; set; } = null;

        /// <summary>
        /// Gets the computed row offset based on page and size.
        /// </summary>
        public int Skip => PageNumber > 0 ? (PageNumber - 1) * PageSize : 0;
    }
}
