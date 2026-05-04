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

public class NotificationDispatchServiceTests
{
    [Fact]
    public async Task DispatchPendingAsync_ShouldSendNotification_AndMarkEventAsSent()
    {
        var updateEventRepository = Substitute.For<IUpdateEventRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var linkRepository = Substitute.For<ILinkRepository>();
        var messageSender = Substitute.For<IOutboxMessageWriter>();
        var formatter = Substitute.For<INotificationFormatter>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<NotificationDispatchService>>();

        var options = Options.Create(new LinkProcessingOptions
        {
            NotificationBatchSize = 10
        });

        var updateEvent = new UpdateEvent
        {
            Id = 1,
            LinkId = 100,
            Status = UpdateEventStatus.Pending,
            Source = "GitHub",
            EventType = "Issue",
            Title = "New issue",
            Author = "octocat",
            Preview = "preview",
            CreatedAt = DateTimeOffset.UtcNow
        };

        updateEventRepository.GetPendingUpdateEventAsync(Arg.Any<PageRequest>(), Arg.Any<CancellationToken>())
            .Returns(new List<UpdateEvent> { updateEvent }, new List<UpdateEvent>());

        linkRepository.GetByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, Link>
            {
                [100] = new Link { Id = 100, Url = "https://github.com/test/repo" }
            });

        subscriptionRepository.GetChatIdsByLinkIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, IReadOnlyCollection<long>>
            {
                [100] = new List<long> { 123456 }
            });

        formatter.Format(updateEvent, "https://github.com/test/repo")
            .Returns("formatted message");

        var service = new NotificationDispatchService(
            updateEventRepository,
            subscriptionRepository,
            linkRepository,
            messageSender,
            formatter,
            unitOfWork,
            options,
            logger);

        await service.DispatchPendingAsync();

        await messageSender.Received(1).WriteAsync(
            Arg.Is<LinkUpdate>(x => x.ChatIds.Contains(123456) && x.Description == "formatted message"),
            Arg.Any<CancellationToken>());

        Assert.Equal(UpdateEventStatus.Sent, updateEvent.Status);

        await updateEventRepository.Received(1)
            .UpdateEventAsync(updateEvent, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DispatchPendingAsync_ShouldMarkFailed_WhenSenderThrows()
    {
        var updateEventRepository = Substitute.For<IUpdateEventRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var linkRepository = Substitute.For<ILinkRepository>();
        var messageSender = Substitute.For<IOutboxMessageWriter>();
        var formatter = Substitute.For<INotificationFormatter>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<NotificationDispatchService>>();

        var options = Options.Create(new LinkProcessingOptions
        {
            NotificationBatchSize = 10
        });

        var updateEvent = new UpdateEvent
        {
            Id = 1,
            LinkId = 100,
            Status = UpdateEventStatus.Pending,
            Source = "GitHub",
            EventType = "Issue",
            Title = "New issue",
            Author = "octocat",
            Preview = "preview",
            CreatedAt = DateTimeOffset.UtcNow
        };

        updateEventRepository.GetPendingUpdateEventAsync(Arg.Any<PageRequest>(), Arg.Any<CancellationToken>())
            .Returns(new List<UpdateEvent> { updateEvent }, new List<UpdateEvent>());

        linkRepository.GetByIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, Link>
            {
                [100] = new Link { Id = 100, Url = "https://github.com/test/repo" }
            });

        subscriptionRepository.GetChatIdsByLinkIdsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, IReadOnlyCollection<long>>
            {
                [100] = new List<long> { 123456 }
            });

        formatter.Format(Arg.Any<UpdateEvent>(), Arg.Any<string>())
            .Returns("formatted message");

        messageSender.WriteAsync(Arg.Any<LinkUpdate>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new Exception("send failed"));

        var service = new NotificationDispatchService(
            updateEventRepository,
            subscriptionRepository,
            linkRepository,
            messageSender,
            formatter,
            unitOfWork,
            options,
            logger);

        await service.DispatchPendingAsync();

        Assert.Equal(UpdateEventStatus.Failed, updateEvent.Status);
    }
}