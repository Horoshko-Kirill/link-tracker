using LinkTracker.AiAgent.Application.InterfacesFactory;
using LinkTracker.AiAgent.Application.InterfacesMetrics;
using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Infrastructure.Factory;
using LinkTracker.AiAgent.Infrastructure.MessageSenders;
using LinkTracker.AiAgent.Infrastructure.Metrics;
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

        services.AddScoped<IMessageSender, KafkaMessageSender>();

        services.AddSingleton<IRedMetrics, RedMetrics>();
        return services;
    }
}