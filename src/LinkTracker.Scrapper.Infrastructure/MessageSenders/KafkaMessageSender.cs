using System.Text.Json;
using System.Text.Json.Nodes;
using Confluent.Kafka;
using LinkTracker.Bot.Contracts.Avro;
using LinkTracker.Bot.Contracts.Avro.Mappers;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.Infrastructure.MessageSenders;

public class KafkaMessageSender : IMessageSender
{
    private readonly IProducer<string, LinkUpdateEvent> _producer;
    private readonly KafkaOptions _options;

    public KafkaMessageSender(IProducer<string, LinkUpdateEvent> producer, IOptions<KafkaOptions> options)
    {
        _producer = producer;
        _options = options.Value;
    }

    public async Task SendAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        var message = new Message<string, LinkUpdateEvent> { Key = linkUpdate.Url, Value = LinkUpdateAvroMappers.ToAvro(linkUpdate) };

        await _producer.ProduceAsync(_options.Topic, message, cancellationToken);
    }
}