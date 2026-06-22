using System.Text.Json;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Infrastructure.Exceptions;
using LinkTracker.Bot.Infrastructure.Kafka.Interfaces;
using LinkTracker.Bot.Infrastructure.Kafka.Result;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Infrastructure.Kafka.Services;

public class LinkUpdateProcessingService : ILinkUpdateProcessingService
{
    private readonly ILinkUpdateMessageProcessor _processor;
    private readonly KafkaConsumerOptions _options;
    private readonly ILogger<LinkUpdateProcessingService> _logger;

    public LinkUpdateProcessingService(
        ILinkUpdateMessageProcessor processor,
        IOptions<KafkaConsumerOptions> options,
        ILogger<LinkUpdateProcessingService> logger)
    {
        _processor = processor;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ProcessingResult> ProcessAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= _options.MaxProcessingAttempts; attempt++)
        {
            try
            {
                await _processor.ProcessAsync(linkUpdate, cancellationToken);
                return ProcessingResult.Success();
            }
            catch (JsonException ex)
            {
                return ProcessingResult.Failed("DeserializationError", ex.Message);
            }
            catch (ValidationException ex)
            {
                return ProcessingResult.Failed("ValidationError", ex.Message);
            }
            catch (Exception ex) when (attempt < _options.MaxProcessingAttempts)
            {
                _logger.LogWarning(
                    ex,
                    "Kafka message processing failed. Attempt {Attempt}/{MaxAttempts}",
                    attempt,
                    _options.MaxProcessingAttempts);

                await Task.Delay(_options.RetryDelayMilliseconds, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Kafka message processing failed after {MaxAttempts} attempts",
                    _options.MaxProcessingAttempts);

                return ProcessingResult.Failed(
                    "ProcessingError",
                    $"Message was not processed after {_options.MaxProcessingAttempts} attempts. {ex.Message}");
            }
        }

        return ProcessingResult.Failed("ProcessingError", "Unknown processing error");
    }
}
