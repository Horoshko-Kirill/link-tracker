using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Scrapper;

public class LinkUpdateServiceTests
{
   [Fact]
    public async Task CheckUpdatesAsync_ShouldBuildReports_WhenThereAreFailedLinks()
    {
        var linkRepository = Substitute.For<ILinkRepository>();
        var reportBuilder = Substitute.For<IReportBuilderService>();
        var logger = Substitute.For<ILogger<LinkUpdateService>>();
        var processor = Substitute.For<ILinkProcessor>();

        var services = new ServiceCollection();
        services.AddScoped(_ => processor);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var options = Options.Create(new LinkProcessingOptions
        {
            BatchSize = 2,
            MaxDegreeOfParallelism = 1
        });

        var links = new List<Link>
        {
            new() { Id = 1, Url = "https://github.com/test/repo1" },
            new() { Id = 2, Url = "https://github.com/test/repo2" }
        };

        linkRepository.GetPageAsync(Arg.Any<PageRequest>(), Arg.Any<CancellationToken>())
            .Returns(links, new List<Link>());

        processor.ProcessAsync(Arg.Is<Link>(x => x.Id == 1), Arg.Any<CancellationToken>())
            .Returns(LinkProcessingResult.Ok());

        processor.ProcessAsync(Arg.Is<Link>(x => x.Id == 2), Arg.Any<CancellationToken>())
            .Returns(LinkProcessingResult.Error(2, "https://github.com/test/repo2", "API unavailable"));

        var service = new LinkUpdateService(
            linkRepository,
            scopeFactory,
            reportBuilder,
            options,
            logger);

        await service.CheckUpdatesAsync();

        await reportBuilder.Received(1).BuildReportsAsync(
            Arg.Is<IReadOnlyCollection<LinkProcessingResult>>(x => x.Count == 1 && x.First().LinkId == 2),
            Arg.Any<DateTimeOffset>(),
            Arg.Any<DateTimeOffset>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CheckUpdatesAsync_ShouldNotBuildReports_WhenAllLinksProcessedSuccessfully()
    {
        var linkRepository = Substitute.For<ILinkRepository>();
        var reportBuilder = Substitute.For<IReportBuilderService>();
        var logger = Substitute.For<ILogger<LinkUpdateService>>();
        var processor = Substitute.For<ILinkProcessor>();

        var services = new ServiceCollection();
        services.AddScoped(_ => processor);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var options = Options.Create(new LinkProcessingOptions
        {
            BatchSize = 2,
            MaxDegreeOfParallelism = 1
        });

        var links = new List<Link>
        {
            new() { Id = 1, Url = "https://github.com/test/repo1" }
        };

        linkRepository.GetPageAsync(Arg.Any<PageRequest>(), Arg.Any<CancellationToken>())
            .Returns(links, new List<Link>());

        processor.ProcessAsync(Arg.Any<Link>(), Arg.Any<CancellationToken>())
            .Returns(LinkProcessingResult.Ok());

        var service = new LinkUpdateService(
            linkRepository,
            scopeFactory,
            reportBuilder,
            options,
            logger);

        await service.CheckUpdatesAsync();

        await reportBuilder.DidNotReceive().BuildReportsAsync(
            Arg.Any<IReadOnlyCollection<LinkProcessingResult>>(),
            Arg.Any<DateTimeOffset>(),
            Arg.Any<DateTimeOffset>(),
            Arg.Any<CancellationToken>());
    }
}
