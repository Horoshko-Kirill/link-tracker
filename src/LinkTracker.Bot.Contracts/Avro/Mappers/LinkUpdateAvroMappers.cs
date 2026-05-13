using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Contracts.Avro.Mappers;

public static class LinkUpdateAvroMappers
{
    public static LinkUpdateEvent ToAvro(LinkUpdate dto)
    {
        return new LinkUpdateEvent
        {
            eventId = Guid.NewGuid().ToString(),
            url = dto.Url ?? string.Empty,
            description = dto.Description,
            tgChatIds = dto.ChatIds
        };
    }

    public static LinkUpdate ToDto(LinkUpdateEvent avro)
    {
        return new LinkUpdate
        {
            EventId = avro.eventId,
            Url = avro.url,
            Description = avro.description,
            ChatIds = avro.tgChatIds.ToList()
        };
    }
}