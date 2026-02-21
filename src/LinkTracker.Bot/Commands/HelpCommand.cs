using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class HelpCommand : ICommand
{
    private readonly ITelegramClient _client;
    private readonly IEnumerable<ICommand> _command;

    public HelpCommand(ITelegramClient client, IEnumerable<ICommand> command)
    {
        _client = client;
        _command = command;
    }
    public string Name => "/help";

    public string Description => "Список команд";

    public async Task ExecuteAsync(long chatId)
    {
        var text = _command
            .Where(c => !string.IsNullOrEmpty(c.Name))
            .Select(e => $"{e.Name} - {e.Description}")
            .ToList();

        var message = string.Join("\n", text);

        await _client.SendMessageAsync(chatId, message);
    }
}
