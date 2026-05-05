using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Infrastructure.Quartz.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class QuartzExtensions
{
    public static IServiceCollection AddScrapperQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration
            .GetSection("LinkProcessing")
            .Get<LinkProcessingOptions>() ?? new LinkProcessingOptions();

        services.Configure<LinkProcessingOptions>(configuration.GetSection("LinkProcessing"));

        services.AddQuartz(q =>
        {
            var linkUpdateJobKey = new JobKey(nameof(LinkUpdateJob));
            q.AddJob<LinkUpdateJob>(opts => opts.WithIdentity(linkUpdateJobKey));
            q.AddTrigger(opts => opts
                .ForJob(linkUpdateJobKey)
                .WithIdentity("LinkUpdateJob-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(options.UpdateInterval)
                    .RepeatForever()));

            var notificationDispatchJobKey = new JobKey(nameof(NotificationDispatchJob));
            q.AddJob<NotificationDispatchJob>(opts => opts.WithIdentity(notificationDispatchJobKey));
            q.AddTrigger(opts => opts
                .ForJob(notificationDispatchJobKey)
                .WithIdentity("NotificationDispatchJob-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(options.NotificationInterval)
                    .RepeatForever()));

            var reportDispatchJobKey = new JobKey(nameof(ReportDispatchJob));
            q.AddJob<ReportDispatchJob>(opts => opts.WithIdentity(reportDispatchJobKey));
            q.AddTrigger(opts => opts
                .ForJob(reportDispatchJobKey)
                .WithIdentity("ReportDispatchJob-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(options.ReportInterval)
                    .RepeatForever()));

            var outboxMessageDispatchJobKey = new JobKey(nameof(OutboxMessageDispatchJob));
            q.AddJob<OutboxMessageDispatchJob>(opts => opts.WithIdentity(outboxMessageDispatchJobKey));
            q.AddTrigger(opts => opts
                .ForJob(outboxMessageDispatchJobKey)
                .WithIdentity("OutboxMessageDispatchJob-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(options.OutboxInterval)
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}