using AuthTask.Application.Common;

namespace AuthTask.Application.Interfaces
{
    /// <summary>
    /// Provides access to request tracking context.
    /// </summary>
    public interface ITrackingContextProvider
    {
        /// <summary>
        /// Gets the current tracking context.
        /// </summary>
        TrackingContext Current { get; }

        /// <summary>
        /// Clears the current tracking context.
        /// </summary>
        void Clear();

        /// <summary>
        /// Initializes tracking context for the current execution flow.
        /// </summary>
        /// <param name="trackingId">Optional explicit tracking identifier.</param>
        /// <param name="isJob">Whether the context belongs to a background job.</param>
        void Initialize(string? trackingId = null, bool isJob = false);

        /// <summary>
        /// Restores tracking context from a previous snapshot.
        /// </summary>
        /// <param name="context">Tracking context snapshot.</param>
        void Restore(TrackingContext context);

        /// <summary>
        /// Sets the current user identifier in the tracking context.
        /// </summary>
        /// <param name="currentUser">Current user identifier.</param>
        void SetContext(string? currentUser);

        /// <summary>
        /// Creates a copy of the current tracking context.
        /// </summary>
        /// <returns>Tracking context snapshot.</returns>
        TrackingContext Snapshot();
    }
}
