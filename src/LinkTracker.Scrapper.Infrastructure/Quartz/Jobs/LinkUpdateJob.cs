using LinkTracker.Scrapper.Application.InterfacesServices;
using Quartz;

namespace LinkTracker.Scrapper.Infrastructure.Quartz.Jobs;

public class LinkUpdateJob : IJob
{
    private readonly ILinkUpdateService _linkUpdateService;

    public LinkUpdateJob(ILinkUpdateService linkUpdateService)
    {
        _linkUpdateService = linkUpdateService;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        await _linkUpdateService.CheckUpdatesAsync(context.CancellationToken);
    }
}
