using System.Diagnostics;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Scrapper.Infrastructure.MessageSenders;

public class FallbackMessageSender : IMessageSender
{
    private readonly HttpMessageSender _httpMessageSender;
    private readonly KafkaMessageSender _kafkaMessageSender;
    private readonly IExternalMetrics _externalMetrics;
    private readonly ILogger<FallbackMessageSender> _logger;

    public FallbackMessageSender(
        ILogger<FallbackMessageSender> logger, 
        KafkaMessageSender kafkaMessageSender, 
        HttpMessageSender httpMessageSender,
        IExternalMetrics externalMetrics)
    {
        _logger = logger;
        _kafkaMessageSender = kafkaMessageSender;
        _httpMessageSender = httpMessageSender;
        _externalMetrics = externalMetrics;
    }
    public async Task SendAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _kafkaMessageSender.SendAsync(linkUpdate, cancellationToken);
            sw.Stop();
            _externalMetrics.ObserveScopeDuration(
                "Ai-Agent",
                "Ai-Agent",
                sw.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Kafka notification transport failed. Switching to HTTP fallback");
            await _httpMessageSender.SendAsync(linkUpdate, cancellationToken);
        }
    }
}