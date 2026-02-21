namespace LinkTracker.Bot.Services;

public class TelegramHostedService : IHostedService
{
    private readonly TelegramHostedService _telegramHostedService;

    public TelegramHostedService(TelegramHostedService telegramHostedService)
    {
        _telegramHostedService = telegramHostedService;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return _telegramHostedService.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
