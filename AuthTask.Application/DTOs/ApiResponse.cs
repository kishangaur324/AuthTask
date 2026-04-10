namespace AuthTask.Application.DTOs
{
    /// <summary>
    /// Standardized API response wrapper.
    /// </summary>
    /// <typeparam name="T">Payload data type.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets successful response data.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the error message when request processing fails.
        /// </summary>
        public string? Error { get; set; }
    }
}
