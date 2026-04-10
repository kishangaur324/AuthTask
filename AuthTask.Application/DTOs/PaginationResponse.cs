namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Generic paginated response payload.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    public class PaginationResponse<T>
    {
        /// <summary>
        /// Gets or sets total record count.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets page items.
        /// </summary>
        public required List<T> Items { get; set; }
    }
}
