namespace AuthTask.Application.Common
{
    /// <summary>
    /// Holds per-request tracking metadata.
    /// </summary>
    public class TrackingContext
    {
        /// <summary>
        /// Gets or sets the tracking identifier.
        /// </summary>
        public string? TrackingId { get; set; }

        /// <summary>
        /// Gets or sets the current user identifier.
        /// </summary>
        public string? CurrentUser { get; set; }
    }
}
