using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Scrapper;

public class ReportDispatchServiceTests
{
    [Fact]
    public async Task DispatchPendingAsync_ShouldSendReport_AndMarkSent()
    {
        var reportRepository = Substitute.For<IChatLinkScanReportRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var messageSender = Substitute.For<IMessageSender>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<ReportDispatchService>>();

        var options = Options.Create(new LinkProcessingOptions
        {
            ReportBatchSize = 10
        });

        var report = new ChatLinkScanReport
        {
            Id = 1,
            ChatId = 10,
            Message = "report message",
            Status = ReportStatus.Pending
        };

        reportRepository.GetPendingAsync(Arg.Any<PageRequest>(), Arg.Any<CancellationToken>())
            .Returns(new List<ChatLinkScanReport> { report }, new List<ChatLinkScanReport>());

        chatRepository.GetByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, Chat>
            {
                [10] = new Chat { Id = 10, ChatId = 123456 }
            });

        var service = new ReportDispatchService(
            reportRepository,
            chatRepository,
            messageSender,
            unitOfWork,
            options,
            logger);

        await service.DispatchPendingAsync();

        await messageSender.Received(1).SendAsync(
            Arg.Is<LinkUpdate>(x => x.ChatIds.Contains(123456) && x.Description == "report message"),
            Arg.Any<CancellationToken>());

        Assert.Equal(ReportStatus.Sent, report.Status);
    }

    [Fact]
    public async Task DispatchPendingAsync_ShouldMarkFailed_WhenSenderThrows()
    {
        var reportRepository = Substitute.For<IChatLinkScanReportRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var messageSender = Substitute.For<IMessageSender>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<ReportDispatchService>>();

        var options = Options.Create(new LinkProcessingOptions
        {
            ReportBatchSize = 10
        });

        var report = new ChatLinkScanReport
        {
            Id = 1,
            ChatId = 10,
            Message = "report message",
            Status = ReportStatus.Pending
        };

        reportRepository.GetPendingAsync(Arg.Any<PageRequest>(), Arg.Any<CancellationToken>())
            .Returns(new List<ChatLinkScanReport> { report }, new List<ChatLinkScanReport>());

        chatRepository.GetByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, Chat>
            {
                [10] = new Chat { Id = 10, ChatId = 123456 }
            });

        messageSender.SendAsync(Arg.Any<LinkUpdate>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new Exception("send failed"));

        var service = new ReportDispatchService(
            reportRepository,
            chatRepository,
            messageSender,
            unitOfWork,
            options,
            logger);

        await service.DispatchPendingAsync();

        Assert.Equal(ReportStatus.Failed, report.Status);
    }
}