namespace LinkTracker.Bot.Dispatching;

/// <summary>
/// Интерфейс диспетчера команд
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Выполняет команду, если находит имя в списке зарегистрированных команд
    /// </summary>
    /// <param name="message"></param>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task DispatchAsync(string message, long chatId, CancellationToken cancellationToken);
}
