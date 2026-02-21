using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

/// <summary>
/// Неизвестная команда для обработки исключительных ситуаций пользовательского ввода
/// </summary>
public class UnknownCommand : ICommand
{
    private readonly ITelegramClient _client;
    public UnknownCommand(ITelegramClient client)
    {
        _client = client;
    }
    public string Name => String.Empty;

    public string Description => String.Empty;

    public async Task ExecuteAsync(long chatId, CancellationToken cancellationToken = default)
    {
        await _client.SendMessageAsync(chatId, "Неизвестная команда. Используйте /help.", cancellationToken);
    }
}
