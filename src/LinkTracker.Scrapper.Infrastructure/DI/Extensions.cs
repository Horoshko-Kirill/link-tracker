using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepository(configuration);

        services.AddSingleton<IGitHubClient, GitHubClient>();
        services.AddSingleton<IStackOverflowClient, StackOverflowClient>();
        services.AddMessageSenders(configuration);
        services.AddScrapperQuartz(configuration);

        var valkeyOptions = configuration.GetSection(ValkeyOptions.SectionName).Get<ValkeyOptions>();

        if (valkeyOptions?.Enabled == true)
        {
            services.AddCached(configuration);
        }

        return services;
    }
}
