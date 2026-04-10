using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;

namespace LinkTracker.Scrapper.Application.Services;

public class ReportBuilderService : IReportBuilderService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IChatLinkScanReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReportFormatter _reportFormatter;

    public ReportBuilderService(
        ISubscriptionRepository subscriptionRepository,
        IChatLinkScanReportRepository reportRepository,
        IUnitOfWork unitOfWork,
        IReportFormatter reportFormatter)
    {
        _subscriptionRepository = subscriptionRepository;
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
        _reportFormatter = reportFormatter;
    }
    
    public async Task BuildReportsAsync(
        IReadOnlyCollection<LinkProcessingResult> failedResults,
        DateTimeOffset scanStartedAt, 
        DateTimeOffset scanFinishedAt, 
        CancellationToken cancellationToken = default)
    {
        if (failedResults.Count == 0)
        {
            return;
        }
        
        var groupedByChatId = new Dictionary<long, List<LinkProcessingResult>>();
        
        
    }
}