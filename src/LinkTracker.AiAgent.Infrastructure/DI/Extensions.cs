using LinkTracker.AiAgent.Application.InterfacesFactory;
using LinkTracker.AiAgent.Infrastructure.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.AiAgent.Infrastructure.DI;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddSummarize(configuration);
        services.AddKafka(configuration);
        return services;
    }
}