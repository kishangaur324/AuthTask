namespace AuthTask.Application.Interfaces
{
    /// <summary>
    /// Exposes request-level metadata to services.
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// Gets the current request identifier.
        /// </summary>
        string? RequestId { get; }
    }
}
