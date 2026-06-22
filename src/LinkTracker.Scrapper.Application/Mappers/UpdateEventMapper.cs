using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Mappers;

public static class UpdateEventMapper
{
    public static UpdateEvent ToDomain(long linkId, UpdateEventDto dto)
    {
        return new UpdateEvent
        {
            LinkId = linkId,
            EventType = dto.EventType,
            Source = dto.Source,
            Title = dto.Title,
            Author = dto.Author,
            CreatedAt = dto.CreatedAt,
            Preview = dto.Preview,
            DetectedAt = DateTimeOffset.UtcNow
        };
    }
}