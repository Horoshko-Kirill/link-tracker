namespace LinkTracker.Bot.Dispatching;

/// <summary>
/// Интерфейс диспетчера команд
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Выполняет команду, если находит имя в списке зарегистрированных команд
    /// </summary>
    /// <param name="name"></param>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task DispatchAsync(string name, long chatId, CancellationToken cancellationToken);
}
