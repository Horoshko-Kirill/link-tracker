using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Domain.Models;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Scrapper;

public class ReportBuilderServiceTests
{
    [Fact]
    public async Task BuildReportsAsync_ShouldCreateAggregatedReportsPerChat()
    {
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var reportRepository = Substitute.For<IChatLinkScanReportRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var formatter = Substitute.For<IReportFormatter>();

        var failedResults = new List<LinkProcessingResult>
        {
            LinkProcessingResult.Error(1, "https://github.com/test/repo1", "error1"),
            LinkProcessingResult.Error(2, "https://github.com/test/repo2", "error2")
        };

        subscriptionRepository.GetChatDbIdsByLinkIdsAsync(
                Arg.Any<IReadOnlyCollection<long>>(),
                Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, IReadOnlyCollection<long>>
            {
                [1] = new List<long> { 10 },
                [2] = new List<long> { 10, 20 }
            });

        formatter.Format(Arg.Any<IReadOnlyCollection<LinkProcessingResult>>(), Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
            .Returns("report");

        var service = new ReportBuilderService(
            subscriptionRepository,
            reportRepository,
            unitOfWork,
            formatter);

        await service.BuildReportsAsync(
            failedResults,
            DateTimeOffset.UtcNow.AddMinutes(-1),
            DateTimeOffset.UtcNow);

        await reportRepository.Received(2)
            .AddAsync(Arg.Any<ChatLinkScanReport>(), Arg.Any<CancellationToken>());

        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BuildReportsAsync_ShouldDoNothing_WhenNoFailedResults()
    {
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var reportRepository = Substitute.For<IChatLinkScanReportRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var formatter = Substitute.For<IReportFormatter>();

        var service = new ReportBuilderService(
            subscriptionRepository,
            reportRepository,
            unitOfWork,
            formatter);

        await service.BuildReportsAsync(
            Array.Empty<LinkProcessingResult>(),
            DateTimeOffset.UtcNow.AddMinutes(-1),
            DateTimeOffset.UtcNow);

        await reportRepository.DidNotReceive()
            .AddAsync(Arg.Any<ChatLinkScanReport>(), Arg.Any<CancellationToken>());

        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}