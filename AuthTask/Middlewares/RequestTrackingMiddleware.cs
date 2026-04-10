using AuthTask.Application.Interfaces;
using Serilog.Context;

namespace AuthTask.Middlewares
{
    /// <summary>
    /// Adds request tracking information to headers and logging scope.
    /// </summary>
    public class RequestTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTrackingMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next middleware delegate.</param>
        public RequestTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Initializes and clears per-request tracking context around the remaining pipeline.
        /// </summary>
        /// <param name="context">Current HTTP context.</param>
        /// <param name="trackingContextProvider">Tracking context provider.</param>
        public async Task InvokeAsync(
            HttpContext context,
            ITrackingContextProvider trackingContextProvider
        )
        {
            trackingContextProvider.Initialize();
            context.Response.Headers.Append(
                "X-Tracking-Id",
                trackingContextProvider.Current.TrackingId
            );

            using (LogContext.PushProperty("RequestId", trackingContextProvider.Current.TrackingId))
            {
                try
                {
                    await _next(context);
                }
                finally
                {
                    trackingContextProvider.Clear();
                }
            }
        }
    }
}
