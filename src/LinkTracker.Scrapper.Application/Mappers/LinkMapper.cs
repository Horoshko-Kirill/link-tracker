using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Mappers;

public static class LinkMapper
{
    public static Link ToDomain(AddLinkRequest request)
    {
        return new Link
        {
            Url = request.Url,
            LastChecked = DateTimeOffset.UtcNow
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

    public static LinkResponse ToResponse(Subscription subscription)
    {
        return new LinkResponse
        {
            ChatId = subscription.ChatId,
            Url = subscription.Link.Url,
            Tags = subscription.Tags.Select(t => t.Name).ToList()
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

    public static ListLinksResponse ToListResponse(List<Subscription> subscriptions)
    {
        return new ListLinksResponse
        {
            Links = subscriptions.Select(subscriptions => ToResponse(subscriptions)).ToList(),

            Size = subscriptions.Count
        };
    }

    public static LinkUpdate ToUpdateRequest(Link link, DateTimeOffset update)
    {
        return new LinkUpdate
        {
            Url = link.Url,
            Description = update.UtcDateTime.AddHours(3).ToString(),
            ChatIds = link.Subscriptions.Select(s => s.Chat.ChatId).ToList()
        };
    }

    
}
