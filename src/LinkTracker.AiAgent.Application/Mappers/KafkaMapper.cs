using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.AiAgent.Application.Mappers;

public static class KafkaMapper
{
    public static RawLinkUpdate ToModel(LinkTracker.Scrapper.Contracts.Avro.LinkUpdateEvent dto)
    {
        return new RawLinkUpdate
        {
            EventId = dto.eventId,
            Url = dto.url,
            Description = dto.description,
            ChatIds = dto.tgChatIds.ToList()
        };

    }

    public static LinkTracker.Bot.Contracts.Avro.LinkUpdateEvent ToDto(ProcessedLinkUpdate model)
    {
        return new LinkTracker.Bot.Contracts.Avro.LinkUpdateEvent
        {
            eventId = model.EventId,
            url = model.Url,
            description = model.Description,
            tgChatIds = model.ChatIds,
            priorityLevel = model.PriorityLevel.ToString()
        };
    }

}