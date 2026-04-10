namespace AuthTask.Application.Enums
{
    /// <summary>
    /// Represents the application-level outcome of an operation.
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// Operation completed successfully with data.
        /// </summary>
        Success,

        /// <summary>
        /// Requested resource was not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Request could not be authenticated.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Operation failed due to an unexpected error.
        /// </summary>
        Failure,

        /// <summary>
        /// Operation completed successfully with no response body.
        /// </summary>
        NoContent,

        /// <summary>
        /// Operation conflicts with current resource state.
        /// </summary>
        Conflict,

        /// <summary>
        /// Request was authenticated but not authorized.
        /// </summary>
        Forbidden,
    }
}
