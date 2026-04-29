using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesServices;

namespace LinkTracker.Scrapper.Infrastructure.MessageSenders;

public class HttpMessageSender : IMessageSender
{
    private readonly IBotClient _botClient;

    public HttpMessageSender(IBotClient botClient)
    {
        _botClient = botClient;
    }

    public Task SendAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        return _botClient.PostUpdateAsync(linkUpdate, cancellationToken);
    }
}