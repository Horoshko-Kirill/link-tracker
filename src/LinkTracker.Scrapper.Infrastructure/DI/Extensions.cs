using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Infrastructure.Quartz.Jobs;
using LinkTracker.Scrapper.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddSingleton<IChatRepository, InMemoryChatRepository>();
        services.AddSingleton<ILinkRepository, InMemoryLinkRepository>();

        services.AddSingleton<IGitHubClient, GitHubClient>();
        services.AddSingleton<IStackOverflowClient, StackOverflowClient>();

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("LinkUpdateJob");
            q.AddJob<LinkUpdateJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("LinkUpdateJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1) 
                    .RepeatForever()
                )
            );
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}
