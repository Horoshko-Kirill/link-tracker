using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Domain.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.Application.Services;

public class ReportDispatchService : IReportDispatchService
{
    private readonly IChatLinkScanReportRepository _chatLinkScanReportRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IOutboxMessageWriter _outboxMessageWriter;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LinkProcessingOptions _options;
    private readonly ILogger<ReportDispatchService> _logger;

    public ReportDispatchService(
        IChatLinkScanReportRepository chatLinkScanReportRepository,
        IChatRepository chatRepository,
        IOutboxMessageWriter outboxMessageWriter,
        IUnitOfWork unitOfWork,
        IOptions<LinkProcessingOptions> options,
        ILogger<ReportDispatchService> logger)
    {
        _chatLinkScanReportRepository = chatLinkScanReportRepository;
        _chatRepository = chatRepository;
        _outboxMessageWriter = outboxMessageWriter;
        _unitOfWork = unitOfWork;
        _options = options.Value;
        _logger = logger;
    }

    public async Task DispatchPendingAsync(CancellationToken cancellationToken = default)
    {
        long lastId = 0;

        while (true)
        {
            var pageRequest = new PageRequest(lastId, _options.ReportBatchSize);
            var reports = await _chatLinkScanReportRepository.GetPendingAsync(pageRequest, cancellationToken);

            if (reports.Count == 0)
            {
                break;
            }

            var chatDbIds = reports
                .Select(x => x.ChatId)
                .Distinct()
                .ToArray();

            var chatsById = await _chatRepository.GetByIdsAsync(chatDbIds, cancellationToken);

            foreach (var report in reports)
            {
                try
                {
                    if (!chatsById.TryGetValue(report.ChatId, out var chat))
                    {
                        report.Status = ReportStatus.Failed;
                        await _chatLinkScanReportRepository.UpdateAsync(report, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        continue;
                    }

                    var notification = new LinkUpdate
                    {
                        Url = $"report:{report.ChatId}",
                        Description = report.Message,
                        ChatIds = [chat.ChatId]
                    };

                    await _outboxMessageWriter.WriteAsync(notification, cancellationToken);

                    report.Status = ReportStatus.Sent;
                    report.SentAt = DateTimeOffset.UtcNow;

                    await _chatLinkScanReportRepository.UpdateAsync(report, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to dispatch scan report {ReportId}", report.Id);

                    report.Status = ReportStatus.Failed;
                    await _chatLinkScanReportRepository.UpdateAsync(report, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            lastId = reports[^1].Id;
        }
    }
}