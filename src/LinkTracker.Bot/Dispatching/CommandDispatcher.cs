using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Commands.Interfaces;

namespace LinkTracker.Bot.Dispatching;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IEnumerable<ICommand> _commands;
    private ICommand _unknownCommand;

    public CommandDispatcher(IEnumerable<ICommand> commands)
    {
        _commands = commands;
        _unknownCommand = SetUnknownCommand(_commands);
    }

    private ICommand SetUnknownCommand(IEnumerable<ICommand> commands)
    {
        return commands.First(e => string.IsNullOrEmpty(e.Name));
    }

    public async Task DispatchAsync(string name, long chatId, CancellationToken cancellationToken = default)
    {

        var command = _commands.FirstOrDefault(c => c.Name == name);

        if (command == null)
        {
            await _unknownCommand.ExecuteAsync(chatId, cancellationToken);
            return;
        }

        await command.ExecuteAsync(chatId, cancellationToken);
    }
}
