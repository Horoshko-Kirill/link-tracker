using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Infrastructure.Options;
using LinkTracker.Scrapper.Options;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace LinkTracker.Scrapper.DI;

public static class BotHttpClientExtensions
{
    public static IServiceCollection AddBotHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IBotClient, BotClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<BotOptions>>().Value;

            client.BaseAddress = new Uri(options.BaseUrl);
        })
        .AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IOptions<ResilienceOptions>>().Value;

            return Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(options.TimeoutSeconds));
        })
        .AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IOptions<ResilienceOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<BotClient>>();

            return HttpPolicyExtensions
                .HandleTransientHttpError()

                .OrResult(response =>
                    options.Retry.RetryableStatusCodes
                        .Contains((int)response.StatusCode))

                .WaitAndRetryAsync(
                    retryCount: options.Retry.MaxAttempts,

                    sleepDurationProvider: _ =>
                        TimeSpan.FromMilliseconds(
                            options.Retry.DelayMilliseconds),

                    onRetry: (outcome, timespan, retryAttempt, _) =>
                    {
                        if (outcome.Exception != null)
                        {
                            logger.LogWarning(
                                outcome.Exception,
                                "Retry {RetryAttempt} after {Delay}ms because of exception",
                                retryAttempt,
                                timespan.TotalMilliseconds);
                        }
                        else
                        {
                            logger.LogWarning(
                                "Retry {RetryAttempt} after {Delay}ms because of status code {StatusCode}",
                                retryAttempt,
                                timespan.TotalMilliseconds,
                                (int?)outcome.Result?.StatusCode);
                        }
                    });
        })
        .AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IOptions<ResilienceOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<BotClient>>();

            return HttpPolicyExtensions
                .HandleTransientHttpError()

                .OrResult(response =>
                    options.Retry.RetryableStatusCodes
                        .Contains((int)response.StatusCode))

                .AdvancedCircuitBreakerAsync(
                    
                    failureThreshold:
                        options.CircuitBreaker.FailureThreshold,
                    
                    samplingDuration:
                        TimeSpan.FromSeconds(
                            options.CircuitBreaker.SamplingDurationSeconds),
                    
                    minimumThroughput:
                        options.CircuitBreaker.MinimumThroughput,

                    durationOfBreak:
                        TimeSpan.FromSeconds(
                            options.CircuitBreaker.DurationOfBreakSeconds),

                    onBreak: (outcome, duration) =>
                    {
                        if (outcome.Exception != null)
                        {
                            logger.LogWarning(
                                outcome.Exception,
                                "Bot circuit breaker OPEN for {Duration} seconds because of exception",
                                duration.TotalSeconds);
                        }
                        else
                        {
                            logger.LogWarning(
                                "Bot circuit breaker OPEN for {Duration} seconds because of status code {StatusCode}",
                                duration.TotalSeconds,
                                (int?)outcome.Result?.StatusCode);
                        }
                    },
                    
                    onReset: () =>
                    {
                        logger.LogInformation("Bot circuit breaker CLOSED");
                    },

                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Bot circuit breaker HALF-OPEN");
                    });
        });

        return services;
    }
}