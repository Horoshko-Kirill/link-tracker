using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class UnknowCommand : ICommand
{
    private readonly ITelegramClient _client;
    public UnknowCommand(ITelegramClient client)
    {
        _client = client; 
    }
    public string Name => String.Empty;

    public string Description => String.Empty;

    public async Task ExecuteAsync(long chatId)
    {
        await _client.SendMessageAsync(chatId, "Неизвестная команда. Используйте /help.");
    }
}
