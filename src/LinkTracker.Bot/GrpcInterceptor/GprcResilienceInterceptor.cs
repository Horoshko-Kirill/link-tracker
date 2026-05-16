using Grpc.Core;
using Grpc.Core.Interceptors;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Polly;

namespace LinkTracker.Bot.ExceptionInterceptor;

public class GrpcResilienceInterceptor : Interceptor
{
    private readonly AsyncPolicy _policy;

    public GrpcResilienceInterceptor(IOptions<ResilienceOptions> options)
    {
        var resilience = options.Value;
        
        var retryPolicy = Policy
            .Handle<RpcException>(ex =>
                ex.StatusCode == StatusCode.Unavailable ||
                ex.StatusCode == StatusCode.DeadlineExceeded)

            .WaitAndRetryAsync(
                resilience.Retry.MaxAttempts,

                _ => TimeSpan.FromMilliseconds(
                    resilience.Retry.DelayMilliseconds));

        var timeoutPolicy = Policy.TimeoutAsync(
            TimeSpan.FromSeconds(
                resilience.TimeoutSeconds));

        var circuitBreakerPolicy = Policy
            .Handle<RpcException>()

            .AdvancedCircuitBreakerAsync(
                resilience.CircuitBreaker.FailureThreshold,

                TimeSpan.FromSeconds(
                    resilience.CircuitBreaker.SamplingDurationSeconds),

                resilience.CircuitBreaker.MinimumThroughput,

                TimeSpan.FromSeconds(
                    resilience.CircuitBreaker.DurationOfBreakSeconds));

        _policy = Policy.WrapAsync(
            retryPolicy,
            circuitBreakerPolicy,
            timeoutPolicy);
    }
    
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var responseTask = _policy.ExecuteAsync(async () =>
        {
            var call = continuation(request, context);

            return await call.ResponseAsync;
        });

        return new AsyncUnaryCall<TResponse>(
            responseTask,
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
    }
}