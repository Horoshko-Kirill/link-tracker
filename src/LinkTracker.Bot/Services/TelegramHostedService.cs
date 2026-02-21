namespace LinkTracker.Bot.Services;

public class TelegramHostedService : IHostedService
{
    private readonly TelegramReceivingService _telegramReceivingService;

    public TelegramHostedService (TelegramReceivingService telegramReceivingService)
    {
        _telegramReceivingService = telegramReceivingService;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return _telegramReceivingService.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
