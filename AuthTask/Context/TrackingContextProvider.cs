using AuthTask.Application.Common;
using AuthTask.Application.Interfaces;

namespace AuthTask.Context
{
    /// <summary>
    /// Provides an async-local request tracking context.
    /// </summary>
    public class TrackingContextProvider : ITrackingContextProvider
    {
        private static readonly AsyncLocal<TrackingContext?> _context = new();

        /// <inheritdoc />
        public TrackingContext Current => _context.Value ??= new TrackingContext();

        /// <inheritdoc />
        public void Initialize(string? trackingId = null, bool isJob = false)
        {
            var current = _context.Value ?? new TrackingContext();

            current.TrackingId = trackingId ?? $"{(isJob ? "J-" : "R-")}{Guid.NewGuid()}";

            _context.Value = current;
        }

        /// <inheritdoc />
        public void SetContext(string? currentUser)
        {
            var current = Current;
            current.CurrentUser = currentUser;
        }

        /// <inheritdoc />
        public TrackingContext Snapshot()
        {
            var current = Current;
            return new TrackingContext
            {
                TrackingId = current.TrackingId,
                CurrentUser = current.CurrentUser,
            };
        }

        /// <inheritdoc />
        public void Restore(TrackingContext context)
        {
            _context.Value = context;
        }

        /// <inheritdoc />
        public void Clear()
        {
            _context.Value = null;
        }
    }
}
