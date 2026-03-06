using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class ListCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IScrapperClient _scrapperClient;

    public ListCommand(ITelegramClient telegramClient, IScrapperClient scrapperClient)
    {
        _telegramClient = telegramClient;
        _scrapperClient = scrapperClient;
    }

    public string Name => "/list";

    public string Description => "Показать отслеживаемые ссылки";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {
        string? tag = null;

        if (args.Length > 0)
        {
            tag = args[0];
        }

        var response = await _scrapperClient.GetLinks(chatId, tag, cancellationToken);

        var links = response.Links;

        if (!links.Any())
        {
            await _telegramClient.SendMessageAsync(chatId, "Список отслеживаемых ссылок пуст", cancellationToken);
            return;
        }

        var message = string.Join("\n", links.Select(x => x.Url));

        await _telegramClient.SendMessageAsync(chatId, message, cancellationToken);
    }
}
