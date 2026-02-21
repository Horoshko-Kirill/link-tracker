namespace LinkTracker.Bot.Services;

public interface ITelegramReceivingService
{
    public Task StartAsync(CancellationToken cancellationToken);
}
