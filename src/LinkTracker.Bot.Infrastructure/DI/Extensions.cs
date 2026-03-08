using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Bot.Infrastructure.DI;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IActionItemRepository, InMemoryActionItemRepository>();
        services.AddSingleton<IProcessRepository, InMemoryProcessRepository>();

        return services;
    }
}
