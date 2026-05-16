using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class GitHubClientExtensions
{
    public static IServiceCollection AddGitHubClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IGitHubClient, GitHubClient>((sp, client) =>
        {
            client.BaseAddress = new Uri("https://api.stackexchange.com/2.3");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("link-tracker-bot");
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
            var logger = sp.GetRequiredService<ILogger<StackOverflowClient>>();

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
            var logger = sp.GetRequiredService<ILogger<StackOverflowClient>>();

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
                                "Github circuit breaker OPEN for {Duration} seconds because of exception",
                                duration.TotalSeconds);
                        }
                        else
                        {
                            logger.LogWarning(
                                "Github circuit breaker OPEN for {Duration} seconds because of status code {StatusCode}",
                                duration.TotalSeconds,
                                (int?)outcome.Result?.StatusCode);
                        }
                    },
                    
                    onReset: () =>
                    {
                        logger.LogInformation(
                            "Github circuit breaker CLOSED");
                    },

                    onHalfOpen: () =>
                    {
                        logger.LogInformation(
                            "Github circuit breaker HALF-OPEN");
                    });
        });
        
        return services;
    }
}