using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Constans;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

/// <summary>
/// Команда /help для вывода возможных команд
/// </summary>
public class HelpCommand : ICommand
{
    private readonly ITelegramClient _client;

    public HelpCommand(ITelegramClient client)
    {
        _client = client;
    }
    public string Name => "/help";

    public string Description => "Список команд";

    public async Task ExecuteAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var message = HelpConstants.constants;

        await _client.SendMessageAsync(chatId, message, cancellationToken);
    }
}
