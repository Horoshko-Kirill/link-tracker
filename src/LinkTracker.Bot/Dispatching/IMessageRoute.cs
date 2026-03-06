namespace LinkTracker.Bot.Dispatching;

public interface IMessageRoute
{
    public Task HandleUpdateAsync(long chatId, string message, CancellationToken cancellationToken = default);
}
