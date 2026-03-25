using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Infrastructure.Database;
using LinkTracker.Scrapper.Infrastructure.Options;
using LinkTracker.Scrapper.Infrastructure.Quartz.Jobs;
using LinkTracker.Scrapper.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Quartz;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Infrastructure.Database.Transaction;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<ScrapperDbContext>((sp, options) =>
        {
            var dbOptions = sp
                .GetRequiredService<IOptions<DatabaseOptions>>()
                .Value;

            options.UseNpgsql(dbOptions.ConnectionString);
        });

        /*services.AddSingleton<IChatRepository, InMemoryChatRepository>();
        services.AddSingleton<ILinkRepository, InMemoryLinkRepository>();*/

        services.AddScoped<IChatRepository, OrmChatRepository>();
        services.AddScoped<ILinkRepository, OrmLinkRepository>();
        services.AddScoped<ISubscriptionRepository, OrmSubscriptionRepository>();
        services.AddScoped<ITagRepository, OrmTagRepository>();

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

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
