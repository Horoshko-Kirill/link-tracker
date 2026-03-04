using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Mappers;

public static class LinkMapper
{
    public static Link ToDomain(long chatId, AddLinkRequest addLinkRequest)
    {
        return new Link
        {
            ChatId = chatId,
            Url = addLinkRequest.Url,
            Tags = addLinkRequest.Tags.Select(t => new Tag { Name = t }).ToList()
        };
    }

    public static LinkResponse ToResponse(Link link)
    {
        return new LinkResponse
        {
            ChatId = link.ChatId,
            Url = link.Url,
            Tags = link.Tags.Select(t => t.Name).ToList()
        };
    }

    public static ListLinksResponse ToListResponse(List<Link> links)
    {
        return new ListLinksResponse
        {
            Links = links.Select(t => ToResponse(t)).ToList(),
            Size = links.Count
        };
    }
}
