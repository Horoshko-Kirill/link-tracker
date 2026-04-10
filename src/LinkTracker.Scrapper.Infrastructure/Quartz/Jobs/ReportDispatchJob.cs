using LinkTracker.Scrapper.Application.InterfacesServices;
using Quartz;

namespace LinkTracker.Scrapper.Infrastructure.Quartz.Jobs;

public class ReportDispatchJob : IJob
{
    private readonly IReportDispatchService _reportDispatchService;

    public ReportDispatchJob(IReportDispatchService reportDispatchService)
    {
        _reportDispatchService = reportDispatchService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        await _reportDispatchService.DispatchPendingAsync(context.CancellationToken);
    }
}