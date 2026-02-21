namespace LinkTracker.Bot.Services;

public class TelegramHostedService : IHostedService
{
    private readonly ITelegramReceivingService _telegramReceivingService;

    public TelegramHostedService (ITelegramReceivingService telegramReceivingService)
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
