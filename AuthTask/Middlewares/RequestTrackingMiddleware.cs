using AuthTask.Application.Interfaces;
using Serilog.Context;

namespace AuthTask.Middlewares
{
    public class RequestTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

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
