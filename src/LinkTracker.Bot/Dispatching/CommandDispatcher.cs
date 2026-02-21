using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Commands.Interfaces;

namespace LinkTracker.Bot.Dispatching;

public class CommandDispatcher
{
    private readonly IEnumerable<ICommand> _commands;
    private readonly ICommand _unknownCommnd;

    public CommandDispatcher(IEnumerable<ICommand> commands, UnknownCommand unknownCommnd)
    {
        _commands = commands;
        _unknownCommnd = unknownCommnd;
    }

    public async Task DispatchAsync(string name, long chatId, CancellationToken cancellationToken = default)
    {

        var command = _commands.FirstOrDefault(c => c.Name == name);

        if (command == null)
        {
            await _unknownCommnd.ExecuteAsync(chatId, cancellationToken);
            return;
        }

        await command.ExecuteAsync(chatId, cancellationToken);
    }

}
