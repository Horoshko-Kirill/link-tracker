using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Mappers;

public static class LinkMapper
{
    public static Link ToDomain(long chatId, AddLinkRequest request)
    {
        return new Link
        {
            Url = request.Url,
            Subscriptions =
            {
                new Subscription
                {
                    ChatId = chatId,
                    Tags = request.Tags.Select(t => new Tag { Name = t }).ToList()
                }
            }
        };
    }
    public static LinkResponse ToResponse(Link link, long chatId)
    {

        var sub = link.Subscriptions.First(s => s.ChatId == chatId);

        return new LinkResponse
        {
            ChatId = chatId,
            Url = link.Url,
            Tags = sub.Tags.Select(t => t.Name).ToList()
        };
    }

    public static ListLinksResponse ToListResponse(List<Link> links, long chatId)
    {
        return new ListLinksResponse
        {
            Links = links.Select(link => ToResponse(link, chatId)).ToList(),

            Size = links.Count
        };
    }

    public static LinkUpdate ToUpdateRequest(Link link, DateTimeOffset update)
    {
        return new LinkUpdate
        {
            Url = link.Url,
            Description = update.UtcDateTime.AddHours(3).ToString(),
            ChatIds = link.Subscriptions.Select(s => s.ChatId).ToList()
        };
    }
}
