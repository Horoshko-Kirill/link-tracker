using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class StartCommand : ICommand
{
    private readonly ITelegramClient _client;

    public StartCommand(ITelegramClient client)
    {
        _client = client;
    }
    public string Name => "/start";

    public string Description => "Начать выполнение";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {
        await _client.SendMessageAsync(chatId, "Добро пожаловать! Используйте /help для списка команд.", cancellationToken);
    }
}
