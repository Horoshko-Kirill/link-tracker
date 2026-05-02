using Confluent.Kafka;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Infrastructure.DI;

public static class KafkaProducerExtensions
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services)
    {
        
        services.AddSingleton<IProducer<string, string>>(sp =>
        {
            var kafkaOptions = sp.GetRequiredService<IOptions<KafkaConsumerOptions>>().Value;
                    
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaOptions.BootstrapServers, 
                Acks = Acks.All, 
                EnableIdempotence = true
            };
                    
            return new ProducerBuilder<string, string>(config).Build();
        });
        
        return services;
    }
}