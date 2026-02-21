namespace LinkTracker.Bot.Dispatching;

public interface ICommandDispatcher
{
    public Task DispatchAsync(string name, long chatId, CancellationToken cancellationToken);
}
