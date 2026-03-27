using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Scrapper;

public class LinkUpdateServiceTests
{
   [Fact]
    public async Task CheckUpdatesAsync_ShouldSendUpdate_AndUpdateLastChecked()
    {
        var linkRepository = Substitute.For<ILinkRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var provider = Substitute.For<IUpdateProvider>();
        var botClient = Substitute.For<IBotClient>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var logger = Substitute.For<ILogger<LinkUpdateService>>();

        var options = Options.Create(new PaginationOptions
        {
            PageSize = 1
        });

        var chatId = 123;
        var linkId = 1;
        long dbChatId = 10;
        var url = "https://example.com";
        var oldLastChecked = new DateTimeOffset(2026, 3, 13, 14, 20, 0, TimeSpan.Zero);
        var newLastUpdate = new DateTimeOffset(2026, 3, 13, 15, 20, 0, TimeSpan.Zero);

        var link = new Link
        {
            Id = linkId,
            Url = "https://example.com",
            LastChecked = oldLastChecked
        };

        var chat = new Chat
        {
            Id = dbChatId,
            ChatId = chatId
        };

        var subscriptions = new List<Subscription>
        {
            new Subscription
            {
                Id = 1,
                ChatId = chatId,
                LinkId = linkId,
                Chat = chat,
                Link = link,
                Tags = new List<Tag>()
            }
        };

        provider.CanHandle(Arg.Any<Uri>()).Returns(true);
        provider.GetLastUpdateAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
            .Returns(newLastUpdate);
        
        linkRepository.GetPageAsync(Arg.Any<PageRequest>(),Arg.Any<CancellationToken>())
            .Returns(new List<Link> { link }, new List<Link>()); 
        
        subscriptionRepository.GetSubscriptionByLinkAsync(linkId, Arg.Any<PageRequest>(), Arg.Any<CancellationToken>())
            .Returns(subscriptions, new List<Subscription>()); 

        botClient.PostUpdateAsync(Arg.Any<LinkUpdate>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        
        var providers = new List<IUpdateProvider> { provider };

        var service = new LinkUpdateService(
            linkRepository,
            providers,
            botClient,
            logger,
            options,
            subscriptionRepository,
            unitOfWork);

        await service.CheckUpdatesAsync();

        await botClient.Received(1)
            .PostUpdateAsync(
                Arg.Is<LinkUpdate>(u =>
                    u.Url == url &&
                    u.ChatIds.Contains(chatId)), 
                Arg.Any<CancellationToken>());

        await linkRepository.Received(1)
            .UpdateLinkAsync(Arg.Is<Link>(l =>
                l.Id == linkId &&
                l.LastChecked == newLastUpdate));

        await unitOfWork.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
