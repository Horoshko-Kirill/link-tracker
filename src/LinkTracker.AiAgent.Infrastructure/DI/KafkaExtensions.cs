using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using LinkTracker.AiAgent.Infrastructure.Options;
using LinkTracker.Bot.Contracts.Avro;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Infrastructure.DI;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.SectionName));
        
        services.AddSingleton<ISchemaRegistryClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<KafkaOptions>>().Value;

            return new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = options.SchemaRegistryUrl
            });
        });
        
        services.AddSingleton<IProducer<string, LinkUpdateEvent>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<KafkaOptions>>().Value;
            var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = options.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true
            };

            return new ProducerBuilder<string, LinkUpdateEvent>(producerConfig)
                .SetValueSerializer(new AvroSerializer<LinkUpdateEvent>(schemaRegistry))
                .Build();
        });
        
        return services;
    }
}