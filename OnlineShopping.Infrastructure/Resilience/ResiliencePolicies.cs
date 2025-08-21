using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace OnlineShopping.Infrastructure.Resilience;

public static class ResiliencePolicies
{
    public static IAsyncPolicy GetCircuitBreakerPolicy(ILogger logger, string serviceName)
    {
        return Policy
            .Handle<Exception>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromSeconds(60),
                minimumThroughput: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    logger.LogWarning("Circuit breaker opened for {ServiceName}. Duration: {Duration}s", 
                        serviceName, duration.TotalSeconds);
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit breaker reset for {ServiceName}", serviceName);
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit breaker is half-open for {ServiceName}", serviceName);
                });
    }

    public static IAsyncPolicy GetRetryPolicy(ILogger logger, string serviceName)
    {
        return Policy
            .Handle<Exception>(ex => IsTransientError(ex))
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning("Retry {RetryCount} for {ServiceName} after {TimeSpan}s. Error: {Error}", 
                        retryCount, serviceName, timeSpan.TotalSeconds, exception.Message);
                });
    }

    public static IAsyncPolicy GetBulkheadPolicy(int maxParallelization = 10, int maxQueuingActions = 50)
    {
        return Policy.BulkheadAsync(maxParallelization, maxQueuingActions);
    }

    public static IAsyncPolicy GetCombinedPolicy(ILogger logger, string serviceName)
    {
        var retry = GetRetryPolicy(logger, serviceName);
        var circuitBreaker = GetCircuitBreakerPolicy(logger, serviceName);
        var bulkhead = GetBulkheadPolicy();

        return Policy.WrapAsync(retry, circuitBreaker, bulkhead);
    }

    private static bool IsTransientError(Exception exception)
    {
        return exception is HttpRequestException ||
               exception is TaskCanceledException ||
               exception is TimeoutException ||
               exception is BrokenCircuitException ||
               (exception.InnerException != null && IsTransientError(exception.InnerException));
    }
}