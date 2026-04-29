using System.Text.Json;
using Confluent.Kafka;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Infrastructure.Clients;

public class LinkUpdateKafkaConsumer : BackgroundService
{
    
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaConsumerOptions _options;
    private readonly ILogger<LinkUpdateKafkaConsumer> _logger;
    
    public LinkUpdateKafkaConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<KafkaConsumerOptions> options,
        ILogger<LinkUpdateKafkaConsumer> logger)
    {
        _scopeFactory = scopeFactory;
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
            try
            {
                var result = consumer.Consume(stoppingToken);

                var linkUpdate = JsonSerializer.Deserialize<LinkUpdate>(result.Message.Value);

                if (linkUpdate is null)
                {
                    throw new InvalidOperationException("Kafka message body is null");
                }

                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<ILinkUpdateHandler>();

                await handler.HandleAsync(linkUpdate, stoppingToken);

                consumer.Commit(result);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while consuming link update from Kafka");
            }
        }

        consumer.Close();
    }
}