using System.Net;
using AuthTask.Helper;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.CircuitBreaker;

namespace AuthTask.Extensions
{
    public static class RetryPolicyExtension
    {
        public static IServiceCollection RegisterHttpClient(this IServiceCollection services)
        {
            // This configuratrion provides retry on transient failures
            // like InternalServerError, Timeout, Network errors

            services
                .AddHttpClient("ApiClient")
                .AddResilienceHandler(
                    "retry-pipeline",
                    (options, context) =>
                    {
                        var retryHandler =
                            context.ServiceProvider.GetRequiredService<IRetryHandler>();

                        options.AddTimeout(TimeSpan.FromSeconds(10));
                        options.AddRetry(
                            new HttpRetryStrategyOptions
                            {
                                MaxRetryAttempts = 3,
                                Delay = TimeSpan.FromSeconds(1),
                                BackoffType = DelayBackoffType.Exponential,
                                UseJitter = true,
                                OnRetry = retryHandler.OnRetry,
                                ShouldHandle = args =>
                                {
                                    var request = args.Outcome.Result?.RequestMessage;
                                    if (request?.Method != HttpMethod.Get)
                                        return ValueTask.FromResult(false);

                                    return ValueTask.FromResult(
                                        args.Outcome.Result?.StatusCode
                                            >= HttpStatusCode.InternalServerError
                                            || args.Outcome.Exception != null
                                    );
                                },
                            }
                        );
                        options.AddCircuitBreaker(
                            new CircuitBreakerStrategyOptions<HttpResponseMessage>
                            {
                                FailureRatio = 0.5, // 50% failure triggers it
                                MinimumThroughput = 4, // atleast 4 requests before evaluating
                                SamplingDuration = TimeSpan.FromSeconds(10),
                                BreakDuration = TimeSpan.FromSeconds(15),
                            }
                        );
                    }
                );

            return services;
        }
    }
}
