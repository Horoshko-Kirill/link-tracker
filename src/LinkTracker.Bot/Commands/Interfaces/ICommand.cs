namespace LinkTracker.Bot.Commands.Interfaces;

/// <summary>
/// Интерфейс команд
/// </summary>
public interface ICommand
{
    string Name { get; }

    string Description { get; }
    Task ExecuteAsync(long chatId, CancellationToken cancellationToken);
}
