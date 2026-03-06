using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddSingleton<IChatRepository, InMemoryChatRepository>();
        services.AddSingleton<ILinkRepository, InMemoryLinkRepository>();

        return services;
    }
}
