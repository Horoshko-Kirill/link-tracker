using LinkTracker.Bot.Infrastructure.Clients;
using LinkTracker.Bot.Infrastructure.Kafka.Interfaces;
using LinkTracker.Bot.Infrastructure.Kafka.Services;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Bot.Infrastructure.DI;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<KafkaConsumerOptions>(configuration.GetSection(KafkaConsumerOptions.SectionName));

        var dbOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>();

        services.AddRepository(dbOptions);

        services.AddTelegramClient();
        services.AddKafkaProducer();
        
        services.AddScoped<ILinkUpdateMessageProcessor, LinkUpdateMessageProcessor>();
        services.AddScoped<ILinkUpdateProcessingService, LinkUpdateProcessingService>();
        services.AddSingleton<IDeadLetterQueueProducer, DeadLetterQueueProducer>();
        
        services.AddHostedService<LinkUpdateKafkaConsumer>();
        return services;
    }
}
