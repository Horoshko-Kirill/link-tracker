using Confluent.Kafka;
using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Infrastructure.Options;
using LinkTracker.Bot.Contracts.Avro;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Infrastructure.MessageSenders;

public class KafkaMessageSender : IMessageSender
{
    private readonly KafkaOptions _options;
    private readonly IProducer<string, LinkUpdateEvent> _producer;
    
    public KafkaMessageSender(IOptions<KafkaOptions> options, IProducer<string, LinkUpdateEvent> producer)
    {
        _options = options.Value;
        _producer = producer;
    }

    public async Task SendAsync(LinkUpdateEvent linkUpdateEvent, CancellationToken cancellationToken = default)
    {
        await _producer.ProduceAsync(
            _options.ProduceTopic,
            new Message<string, LinkTracker.Bot.Contracts.Avro.LinkUpdateEvent> { Key = linkUpdateEvent.eventId.ToString(), Value = linkUpdateEvent }, cancellationToken);
    }
}