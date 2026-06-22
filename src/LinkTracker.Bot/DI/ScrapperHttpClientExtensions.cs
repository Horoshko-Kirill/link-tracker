using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Infrastructure.Clients;
using LinkTracker.Bot.Infrastructure.Options;
using LinkTracker.Bot.Options;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace LinkTracker.Bot.DI;

public static class ScrapperHttpClientExtensions
{
    public static IServiceCollection AddScrapperHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IScrapperClient, ScrapperClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ScrapperOptions>>().Value;

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
            var logger = sp.GetRequiredService<ILogger<ScrapperClient>>();

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
            var logger = sp.GetRequiredService<ILogger<ScrapperClient>>();

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
                                "Scrapper circuit breaker OPEN for {Duration} seconds because of exception",
                                duration.TotalSeconds);
                        }
                        else
                        {
                            logger.LogWarning(
                                "Scrapper circuit breaker OPEN for {Duration} seconds because of status code {StatusCode}",
                                duration.TotalSeconds,
                                (int?)outcome.Result?.StatusCode);
                        }
                    },

                    onReset: () =>
                    {
                        logger.LogInformation("Scrapper circuit breaker CLOSED");
                    },

                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Scrapper circuit breaker HALF-OPEN");
                    });
        });

        return services;
    }
}