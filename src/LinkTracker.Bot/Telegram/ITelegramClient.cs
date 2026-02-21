using LinkTracker.Bot.Commands.Interfaces;
using Telegram.Bot.Types;

namespace LinkTracker.Bot.Telegram;

public interface ITelegramClient
{
    Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken);
    Task SetCommandsAsync(IEnumerable<ICommand> commands, CancellationToken cancellationToken);
}
