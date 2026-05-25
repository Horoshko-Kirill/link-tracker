using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Mappers;
using LinkTracker.AiAgent.Infrastructure.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Infrastructure.Kafka;

public class KafkaWorker : BackgroundService
{
    private readonly KafkaOptions _options;
    private readonly ISchemaRegistryClient _schemaRegistryClient;
    private readonly IPipeline _pipeline;
    private readonly IGroupingService _groupingService;
    private readonly ILogger<KafkaWorker> _logger;

    public KafkaWorker(
        IOptions<KafkaOptions> options,
        ISchemaRegistryClient schemaRegistryClient,
        IPipeline pipeline,
        IGroupingService groupingService,
        ILogger<KafkaWorker> logger)
    {
        _options = options.Value;
        _schemaRegistryClient = schemaRegistryClient;
        _pipeline = pipeline;
        _groupingService = groupingService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, LinkTracker.Scrapper.Contracts.Avro.LinkUpdateEvent>(config)
            .SetValueDeserializer(new AvroDeserializer<LinkTracker.Scrapper.Contracts.Avro.LinkUpdateEvent>(_schemaRegistryClient).AsSyncOverAsync())
            .Build();

        consumer.Subscribe(_options.ConsumerTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var msg = consumer.Consume(stoppingToken);

                var processedLink = await _pipeline.ProcessAsync(KafkaMapper.ToModel(msg.Message.Value), stoppingToken);

                if (processedLink is null)
                {
                    consumer.Commit(msg);
                    continue;
                }

                await _groupingService.AddAsync(processedLink);
                
                consumer.Commit(msg);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume failed");
            }
            catch (ProduceException<string, LinkTracker.Bot.Contracts.Avro.LinkUpdateEvent> ex)
            {
                _logger.LogError(ex, "Kafka produce failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected worker error");
            }
        }

        consumer.Close();
    }
}