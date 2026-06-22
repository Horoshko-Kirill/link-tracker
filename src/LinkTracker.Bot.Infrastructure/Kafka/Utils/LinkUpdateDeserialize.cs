using System.Text.Json;
using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Infrastructure.Kafka.Utils;

public static class LinkUpdateDeserialize
{
    public static LinkUpdate Deserialize(string message)
    {
        var linkUpdate = JsonSerializer.Deserialize<LinkUpdate>(message);

        if (linkUpdate is null)
        {
            throw new JsonException("Kafka message body is null");
        }

        return linkUpdate;
    }
}