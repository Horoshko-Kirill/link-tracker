using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.InterfacesServices;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Scrapper.Infrastructure.MessageSenders;

public class FallbackMessageSender : IMessageSender
{
    private readonly HttpMessageSender _httpMessageSender;
    private readonly KafkaMessageSender _kafkaMessageSender;
    private readonly ILogger<FallbackMessageSender> _logger;

    public FallbackMessageSender(ILogger<FallbackMessageSender> logger, KafkaMessageSender kafkaMessageSender, HttpMessageSender httpMessageSender)
    {
        _logger = logger;
        _kafkaMessageSender = kafkaMessageSender;
        _httpMessageSender = httpMessageSender;
    }
    public async Task SendAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        try
        {
            await _httpMessageSender.SendAsync(linkUpdate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP notification transport failed. Switching to Kafka fallback");
            await _kafkaMessageSender.SendAsync(linkUpdate, cancellationToken);
        }
    }
}