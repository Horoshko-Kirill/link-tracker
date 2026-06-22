using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesMetrics;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Application.Services;

public class LinkUpdateHandler : ILinkUpdateHandler
{
    private readonly ITelegramClient _telegramClient;
    private readonly INotificationMetrics _metrics;

    public LinkUpdateHandler(ITelegramClient telegramClient, INotificationMetrics metrics)
    {
        _telegramClient = telegramClient;
        _metrics = metrics;
    }

    public async Task HandleAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        foreach (var chatId in linkUpdate.ChatIds)
        {
            await _telegramClient.SendMessageAsync(chatId, linkUpdate.Description, cancellationToken);
            _metrics.IncSent();
        }

    }
}