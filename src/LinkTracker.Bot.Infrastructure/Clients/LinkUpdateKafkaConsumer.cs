using System.Text.Json;
using Confluent.Kafka;
using LinkTracker.Bot.Application.InterfacesServices;
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
    private readonly ILogger<LinkUpdateKafkaConsumer> _logger;
    
    public LinkUpdateKafkaConsumer(
        ILinkUpdateProcessingService processingService,
        IDeadLetterQueueProducer deadLetterQueueProducer,
        IOptions<KafkaConsumerOptions> options,
        ILogger<LinkUpdateKafkaConsumer> logger)
    {
        _processingService = processingService;
        _deadLetterQueueProducer = deadLetterQueueProducer;
        _options = options.Value;
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

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_options.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<string, string>? result = null;
            
            try
            {
                result = consumer.Consume(stoppingToken);

                var processingResult = await _processingService.ProcessAsync(
                    result.Message.Value,
                    stoppingToken);
                
                if (!processingResult.IsSuccess)
                {
                    await _deadLetterQueueProducer.SendAsync(
                        result,
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
                    await _deadLetterQueueProducer.SendAsync(
                        result,
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