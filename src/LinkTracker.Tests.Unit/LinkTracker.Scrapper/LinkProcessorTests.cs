using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Scrapper;

public class LinkProcessorTests
{
     [Fact]
    public async Task ProcessAsync_ShouldSaveEventsAndUpdateLastChecked()
    {
        var linkRepository = Substitute.For<ILinkRepository>();
        var updateEventRepository = Substitute.For<IUpdateEventRepository>();
        var provider = Substitute.For<IUpdateProvider>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<LinkProcessor>>();

        var link = new Link
        {
            Id = 1,
            Url = "https://github.com/test/repo",
            LastChecked = new DateTimeOffset(2026, 4, 10, 10, 0, 0, TimeSpan.Zero)
        };

        var newEventTime = new DateTimeOffset(2026, 4, 10, 11, 0, 0, TimeSpan.Zero);

        provider.CanHandle(Arg.Any<Uri>()).Returns(true);
        provider.GetNewEventsAsync(Arg.Any<Uri>(), Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>())
            .Returns(new List<UpdateEventDto>
            {
                new()
                {
                    Source = "GitHub",
                    EventType = "Issue",
                    Title = "New issue",
                    Author = "octocat",
                    CreatedAt = newEventTime,
                    Preview = "preview"
                }
            });

        var transaction = Substitute.For<IScrapperTransaction>();
        unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>())
            .Returns(transaction);

        var service = new LinkProcessor(
            linkRepository,
            updateEventRepository,
            new[] { provider },
            unitOfWork,
            logger);

        var result = await service.ProcessAsync(link);

        await updateEventRepository.Received(1)
            .AddUpdateEventAsync(Arg.Any<UpdateEvent>(), Arg.Any<CancellationToken>());

        await linkRepository.Received(1)
            .UpdateLastCheckedAsync(link.Id, newEventTime, Arg.Any<CancellationToken>());

        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_ShouldReturnError_WhenProviderNotFound()
    {
        var linkRepository = Substitute.For<ILinkRepository>();
        var updateEventRepository = Substitute.For<IUpdateEventRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<LinkProcessor>>();

        var service = new LinkProcessor(
            linkRepository,
            updateEventRepository,
            Array.Empty<IUpdateProvider>(),
            unitOfWork,
            logger);

        var link = new Link
        {
            Id = 1,
            Url = "https://unknown-host.com/item/1"
        };

        var result = await service.ProcessAsync(link);
        
        Assert.Equal(link.Id, result.LinkId);
    }

    [Fact]
    public async Task ProcessAsync_ShouldReturnError_WhenProviderThrows()
    {
        var linkRepository = Substitute.For<ILinkRepository>();
        var updateEventRepository = Substitute.For<IUpdateEventRepository>();
        var provider = Substitute.For<IUpdateProvider>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<LinkProcessor>>();

        provider.CanHandle(Arg.Any<Uri>()).Returns(true);
        provider.GetNewEventsAsync(Arg.Any<Uri>(), Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>())
            .Returns<Task<IReadOnlyCollection<UpdateEventDto>>>(_ => throw new Exception("boom"));

        var service = new LinkProcessor(
            linkRepository,
            updateEventRepository,
            new[] { provider },
            unitOfWork,
            logger);

        var link = new Link
        {
            Id = 1,
            Url = "https://github.com/test/repo",
            LastChecked = DateTimeOffset.UtcNow
        };

        var result = await service.ProcessAsync(link);

        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}