using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Domain.Models;

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
        
        var uniqueLinkIds = failedResults
            .Select(x => x.LinkId)
            .Distinct()
            .ToArray();
        
        var chatIdsByLinkId = await _subscriptionRepository.GetChatIdsByLinkIdsAsync(
            uniqueLinkIds,
            cancellationToken);
        
        var groupedByChatId = new Dictionary<long, List<LinkProcessingResult>>();
        
        foreach (var failedResult in failedResults)
        {
            if (!chatIdsByLinkId.TryGetValue(failedResult.LinkId, out var chatIds))
            {
                continue;
            }

            foreach (var chatId in chatIds)
            {
                if (!groupedByChatId.TryGetValue(chatId, out var list))
                {
                    list = [];
                    groupedByChatId[chatId] = list;
                }

                list.Add(failedResult);
            }
        }

        foreach (var (chatId, chatFailedLinks) in groupedByChatId)
        {
            var message = _reportFormatter.Format(chatFailedLinks, scanStartedAt, scanFinishedAt);

            var report = new ChatLinkScanReport
            {
                ChatId = chatId,
                ScanStartedAt = scanStartedAt,
                ScanFinishedAt = scanFinishedAt,
                FailedCount = chatFailedLinks.Count,
                Message = message
            };

            await _reportRepository.AddAsync(report, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}