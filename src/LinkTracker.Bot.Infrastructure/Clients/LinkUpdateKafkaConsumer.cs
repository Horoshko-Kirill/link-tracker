using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Contracts.Avro;
using LinkTracker.Bot.Contracts.Avro.Mappers;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Infrastructure.Kafka.Interfaces;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
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
    
    public LinkUpdateKafkaConsumer(
        ILinkUpdateProcessingService processingService,
        IDeadLetterQueueProducer deadLetterQueueProducer,
        IOptions<KafkaConsumerOptions> options,
        ISchemaRegistryClient schemaRegistryClient,
        ILogger<LinkUpdateKafkaConsumer> logger)
    {
        _processingService = processingService;
        _deadLetterQueueProducer = deadLetterQueueProducer;
        _options = options.Value;
        _schemaRegistryClient = schemaRegistryClient;
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

                var processingResult = await _processingService.ProcessAsync(
                    linkUpdate,
                    stoppingToken);
                
                if (!processingResult.IsSuccess)
                {
                    await _deadLetterQueueProducer.SendAsync(
                        result.Message.Key,
                        linkUpdate,
                        processingResult.ErrorType!,
                        processingResult.ErrorMessage!,
                        stoppingToken);
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