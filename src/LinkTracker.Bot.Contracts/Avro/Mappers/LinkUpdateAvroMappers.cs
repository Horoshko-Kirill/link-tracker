using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Contracts.Enums;

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
            tgChatIds = dto.ChatIds,
            priorityLevel = dto.PriorityLevel.ToString()
        };
    }

    public static LinkUpdate ToDto(LinkUpdateEvent avro)
    {
        return new LinkUpdate
        {
            EventId = avro.eventId,
            Url = avro.url,
            Description = avro.description,
            PriorityLevel = Enum.TryParse(avro.priorityLevel, true, out PriorityLevel parsed) ? parsed : PriorityLevel.Medium,
            ChatIds = avro.tgChatIds.ToList()
        };
    }
}