using LinkTracker.Scrapper.Application.InterfacesServices;
using Quartz;

namespace LinkTracker.Scrapper.Infrastructure.Quartz.Jobs;

public class OutboxMessageDispatchJob : IJob
{
    private readonly IOutboxMessageDispatchService _outboxMessageDispatchService;

    public OutboxMessageDispatchJob(IOutboxMessageDispatchService outboxMessageDispatchService)
    {
        _outboxMessageDispatchService = outboxMessageDispatchService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        await _outboxMessageDispatchService.DispatchPendingAsync(context.CancellationToken);
    }
}