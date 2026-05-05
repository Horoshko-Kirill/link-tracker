using Confluent.Kafka;
using Confluent.SchemaRegistry;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Infrastructure.DI;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services)
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
        
        services.AddSingleton<ISchemaRegistryClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<KafkaConsumerOptions>>().Value;

            return new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = options.SchemaRegistryUrl
            });
        });
        
        return services;
    }
}