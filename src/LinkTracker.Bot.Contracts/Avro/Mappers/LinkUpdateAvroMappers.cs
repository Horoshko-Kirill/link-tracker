using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Contracts.Avro.Mappers;

public static class LinkUpdateAvroMappers
{
    public static LinkUpdateEvent ToAvro(LinkUpdate dto)
    {
        return new LinkUpdateEvent
        {
            url = dto.Url ?? string.Empty,
            description = dto.Description,
            tgChatIds = dto.ChatIds
        };
    }

    public static LinkUpdate ToDto(LinkUpdateEvent avro)
    {
        return new LinkUpdate
        {
            Url = avro.url,
            Description = avro.description,
            ChatIds = avro.tgChatIds.ToList()
        };
    }
}