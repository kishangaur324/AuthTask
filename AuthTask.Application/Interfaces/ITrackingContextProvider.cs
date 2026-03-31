using AuthTask.Application.Common;

namespace AuthTask.Application.Interfaces
{
    public interface ITrackingContextProvider
    {
        TrackingContext Current { get; }

        void Clear();
        void Initialize(string? trackingId = null, bool isJob = false);
        void Restore(TrackingContext context);
        void SetContext(string? currentUser);
        TrackingContext Snapshot();
    }
}
