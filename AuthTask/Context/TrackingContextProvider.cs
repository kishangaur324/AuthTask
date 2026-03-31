using AuthTask.Application.Common;
using AuthTask.Application.Interfaces;

namespace AuthTask.Context
{
    public class TrackingContextProvider : ITrackingContextProvider
    {
        private static readonly AsyncLocal<TrackingContext?> _context = new();

        public TrackingContext Current => _context.Value ??= new TrackingContext();

        public void Initialize(string? trackingId = null, bool isJob = false)
        {
            var current = _context.Value ?? new TrackingContext();

            current.TrackingId = trackingId ?? $"{(isJob ? "J-" : "R-")}{Guid.NewGuid()}";

            _context.Value = current;
        }

        public void SetContext(string? currentUser)
        {
            var current = Current;
            current.CurrentUser = currentUser;
        }

        public TrackingContext Snapshot()
        {
            var current = Current;
            return new TrackingContext
            {
                TrackingId = current.TrackingId,
                CurrentUser = current.CurrentUser,
            };
        }

        public void Restore(TrackingContext context)
        {
            _context.Value = context;
        }

        public void Clear()
        {
            _context.Value = null;
        }
    }
}
