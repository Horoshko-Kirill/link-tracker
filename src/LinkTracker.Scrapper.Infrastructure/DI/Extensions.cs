using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddSingleton<IChatRepository, InMemoryChatRepository>();
        services.AddSingleton<ILinkRepository, InMemoryLinkRepository>();

        services.AddSingleton<IGitHubClient, GitHubClient>();
        services.AddSingleton<IStackOverflowClient, StackOverflowClient>();

        return services;
    }
}
