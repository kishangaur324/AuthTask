using Polly.Retry;

namespace AuthTask.Helper
{
    public class RetryHandler : IRetryHandler
    {
        private readonly ILogger<RetryHandler> _logger;

        public RetryHandler(ILogger<RetryHandler> logger)
        {
            _logger = logger;
        }

        public ValueTask OnRetry(OnRetryArguments<HttpResponseMessage> args)
        {
            var reason =
                args.Outcome.Exception?.Message ?? args.Outcome.Result?.StatusCode.ToString();

            _logger.LogWarning(
                "Retry {Attempt} due to {Reason}. Next delay: {Delay}ms",
                args.AttemptNumber,
                reason,
                args.RetryDelay.TotalMilliseconds
            );

            return ValueTask.CompletedTask;
        }
    }
}
