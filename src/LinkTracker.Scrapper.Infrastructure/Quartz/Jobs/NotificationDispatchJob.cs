using LinkTracker.Scrapper.Application.InterfacesServices;
using Quartz;

namespace LinkTracker.Scrapper.Infrastructure.Quartz.Jobs;

public class NotificationDispatchJob : IJob
{
    private readonly INotificationDispatchService _notificationDispatchService;

    public NotificationDispatchJob(INotificationDispatchService notificationDispatchService)
    {
        _notificationDispatchService = notificationDispatchService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _notificationDispatchService.DispatchPendingAsync(context.CancellationToken);
    }
}