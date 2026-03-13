using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Scrapper;

public class LinkUpdateServiceTests
{
    [Fact]
    public async Task CheckUpdatesAsync_ShouldBeUpdateLink()
    {
        var linkRepository = Substitute.For<ILinkRepository>();

        var provider = Substitute.For<IUpdateProvider>();
        provider.CanHandle(Arg.Any<Uri>()).Returns(true);

        var chatId = 123;

        var links = new List<Link>
        {
            new Link
            {
                Url = "https://1",
                Subscriptions = new List<Subscription> {
                    new Subscription {
                        ChatId = chatId,
                    }
                },
                LastChecked = new DateTimeOffset(2026, 3, 13, 14, 20, 0, TimeSpan.Zero)
            }
        };

        provider.GetLastUpdateAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>()).Returns(new DateTimeOffset(2026, 3, 13, 15, 20, 0, TimeSpan.Zero));

        var providers = new List<IUpdateProvider> { provider };
        var client = Substitute.For<IBotClient>();
        var logger = Substitute.For<ILogger<LinkUpdateService>>();

        linkRepository.GetAllLinksAsync().Returns(links);

        var linkUpdateService = new LinkUpdateService(linkRepository, providers, client, logger);

        await linkUpdateService.CheckUpdatesAsync();

        await client.Received(1).PostUpdateAsync(Arg.Any<LinkUpdate>(), Arg.Any<CancellationToken>());
    }
}
