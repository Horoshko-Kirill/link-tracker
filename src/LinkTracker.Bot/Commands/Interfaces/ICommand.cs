namespace LinkTracker.Bot.Commands.Interfaces;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync(long chatId, CancellationToken cancellationToken);
}
