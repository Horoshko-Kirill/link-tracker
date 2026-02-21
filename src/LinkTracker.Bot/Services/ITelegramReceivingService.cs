namespace LinkTracker.Bot.Services;

/// <summary>
/// Интерфейс сервиса для передачи команд телеграмму и назначения обработчика обновлений
/// </summary>
public interface ITelegramReceivingService
{
    public Task StartAsync(CancellationToken cancellationToken);
}
