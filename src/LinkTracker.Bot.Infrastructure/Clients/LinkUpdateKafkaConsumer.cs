using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Contracts.Avro;
using LinkTracker.Bot.Contracts.Avro.Mappers;
using LinkTracker.Bot.Infrastructure.Kafka.Interfaces;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Infrastructure.Clients;

public class LinkUpdateKafkaConsumer : BackgroundService
{
    private readonly ILinkUpdateProcessingService _processingService;
    private readonly IDeadLetterQueueProducer _deadLetterQueueProducer;
    private readonly KafkaConsumerOptions _options;
    private readonly ISchemaRegistryClient _schemaRegistryClient;
    private readonly ILogger<LinkUpdateKafkaConsumer> _logger;
    private readonly IEventDeduplicator _deduplicator;

    public LinkUpdateKafkaConsumer(
        ILinkUpdateProcessingService processingService,
        IDeadLetterQueueProducer deadLetterQueueProducer,
        IOptions<KafkaConsumerOptions> options,
        ISchemaRegistryClient schemaRegistryClient,
        ILogger<LinkUpdateKafkaConsumer> logger,
        IEventDeduplicator deduplicator)
    {
        _processingService = processingService;
        _deadLetterQueueProducer = deadLetterQueueProducer;
        _options = options.Value;
        _schemaRegistryClient = schemaRegistryClient;
        _logger = logger;
        _deduplicator = deduplicator;
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

        using var consumer = new ConsumerBuilder<string, LinkUpdateEvent>(config)
            .SetValueDeserializer(new AvroDeserializer<LinkUpdateEvent>(_schemaRegistryClient).AsSyncOverAsync())
            .Build();
        consumer.Subscribe(_options.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<string, LinkUpdateEvent>? result = null;

            try
            {
                result = consumer.Consume(stoppingToken);

                var linkUpdate = LinkUpdateAvroMappers.ToDto(result.Message.Value);
                
                var alreadyProcessed = await _deduplicator.IsProcessedAsync(linkUpdate.EventId);
                
                if (alreadyProcessed)
                {
                    _logger.LogInformation(
                        "Duplicate skipped {EventId}",
                        linkUpdate.EventId);

                    consumer.Commit(result);

                    continue;
                }

                var processingResult = await _processingService.ProcessAsync(linkUpdate, stoppingToken);

                if (!processingResult.IsSuccess)
                {
                    await _deadLetterQueueProducer.SendAsync(
                        result.Message.Key,
                        linkUpdate,
                        processingResult.ErrorType!,
                        processingResult.ErrorMessage!,
                        stoppingToken);
                }
                else
                {
                    await _deduplicator.MarkProcessedAsync(linkUpdate.EventId);
                }

                consumer.Commit(result);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while consuming link update from Kafka");

                if (result is not null)
                {
                    var linkUpdate = LinkUpdateAvroMappers.ToDto(result.Message.Value);

                    await _deadLetterQueueProducer.SendAsync(
                        result.Message.Key,
                        linkUpdate,
                        "UnexpectedError",
                        ex.Message,
                        stoppingToken);

                    consumer.Commit(result);
                }
            }
        }

        consumer.Close();
    }
}