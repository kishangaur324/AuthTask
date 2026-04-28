using Polly.Retry;

namespace AuthTask.Helper
{
    public interface IRetryHandler
    {
        ValueTask OnRetry(OnRetryArguments<HttpResponseMessage> args);
    }
}