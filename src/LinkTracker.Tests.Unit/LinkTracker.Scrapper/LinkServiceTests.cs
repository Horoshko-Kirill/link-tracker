using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Scrapper;

public class LinkServiceTests
{
    private static LinkService CreateService(
        ILinkRepository? linkRepository = null,
        IChatRepository? chatRepository = null,
        ISubscriptionRepository? subscriptionRepository = null,
        IUnitOfWork? unitOfWork = null,
        int pageSize = 2)
    {
        return new LinkService(
            linkRepository ?? Substitute.For<ILinkRepository>(),
            chatRepository ?? Substitute.For<IChatRepository>(),
            subscriptionRepository ?? Substitute.For<ISubscriptionRepository>(),
            unitOfWork ?? Substitute.For<IUnitOfWork>(),
            Options.Create(new PaginationOptions { PageSize = pageSize })
        );
    }

    private static IScrapperTransaction CreateTransaction()
    {
        return Substitute.For<IScrapperTransaction>();
    }

    [Fact]
    public async Task AddLinkAsync_ShouldAddNewLinkAndSubscription()
    {
        var chatId = 123;
        var request = new AddLinkRequest
        {
            Url = "https://example.com",
            Tags = new List<string> { "news" }
        };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var transaction = CreateTransaction();

        var chat = new Chat { Id = 10, ChatId = chatId };

        chatRepository.GetChatByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(chat);

        subscriptionRepository.ExistsAsync(chatId, request.Url, Arg.Any<CancellationToken>())
            .Returns(false);

        linkRepository.GetByUrlAsync(request.Url, Arg.Any<CancellationToken>())
            .Returns((Link?)null);

        unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>())
            .Returns(transaction);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        var result = await service.AddLinkAsync(chatId, request);

        await linkRepository.Received(1)
            .AddLinkAsync(
                Arg.Is<Link>(x => x.Url == request.Url),
                Arg.Any<CancellationToken>());

        await subscriptionRepository.Received(1)
            .AddSubscriptionAsync(
                Arg.Is<Subscription>(s => s.ChatId == chat.Id),
                Arg.Any<CancellationToken>());

        await unitOfWork.Received(2)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await transaction.Received(1)
            .CommitAsync(Arg.Any<CancellationToken>());

        Assert.Equal(chatId, result.ChatId);
        Assert.Equal(request.Url, result.Url);
        Assert.Contains("news", result.Tags);
    }

    [Fact]
    public async Task AddLinkAsync_ShouldAddOnlySubscription_WhenLinkAlreadyExists()
    {
        var chatId = 123;
        var request = new AddLinkRequest
        {
            Url = "https://example.com",
            Tags = new List<string> { "dev" }
        };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var transaction = CreateTransaction();

        var chat = new Chat { Id = 10, ChatId = chatId };
        var existingLink = new Link { Id = 77, Url = request.Url };

        chatRepository.GetChatByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(chat);

        subscriptionRepository.ExistsAsync(chatId, request.Url, Arg.Any<CancellationToken>())
            .Returns(false);

        linkRepository.GetByUrlAsync(request.Url, Arg.Any<CancellationToken>())
            .Returns(existingLink);

        unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>())
            .Returns(transaction);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        var result = await service.AddLinkAsync(chatId, request);

        await linkRepository.DidNotReceive()
            .AddLinkAsync(Arg.Any<Link>(), Arg.Any<CancellationToken>());

        await subscriptionRepository.Received(1)
            .AddSubscriptionAsync(
                Arg.Is<Subscription>(s => s.ChatId == chat.Id && s.LinkId == existingLink.Id),
                Arg.Any<CancellationToken>());

        await unitOfWork.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await transaction.Received(1)
            .CommitAsync(Arg.Any<CancellationToken>());

        Assert.Equal(request.Url, result.Url);
    }

    [Fact]
    public async Task AddLinkAsync_ShouldThrowBadRequestException_WhenChatIdInvalid()
    {
        var chatId = 0;
        var request = new AddLinkRequest { Url = "https://example.com", Tags = new List<string>() };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        await Assert.ThrowsAsync<BadRequestException>(() => service.AddLinkAsync(chatId, request));

        await unitOfWork.DidNotReceive()
            .BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddLinkAsync_ShouldThrowNotFoundException_WhenChatDoesNotExist()
    {
        var chatId = 123L;
        var request = new AddLinkRequest { Url = "https://example.com", Tags = new List<string>() };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        chatRepository.GetChatByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns((Chat?)null);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(() => service.AddLinkAsync(chatId, request));

        await subscriptionRepository.DidNotReceive()
            .ExistsAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddLinkAsync_ShouldThrowConflictException_WhenSubscriptionAlreadyExists()
    {
        var chatId = 123;
        var request = new AddLinkRequest { Url = "https://example.com", Tags = new List<string>() };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        chatRepository.GetChatByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(new Chat { Id = 10, ChatId = chatId });

        subscriptionRepository.ExistsAsync(chatId, request.Url, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        await Assert.ThrowsAsync<ConflictException>(() => service.AddLinkAsync(chatId, request));

        await unitOfWork.DidNotReceive()
            .BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetLinksAsync_ShouldReturnLinks()
    {
        var chatId = 123;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        chatRepository.ChatExistByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(true);

        var firstPage = new List<Subscription>
        {
            new Subscription
            {
                Id = 1,
                ChatId = chatId,
                LinkId = 100,
                Link = new Link
                {
                    Id = 100,
                    Url = "https://example.com"
                },
                Tags = new List<Tag>
                {
                    new Tag { Name = "news" }
                }
            }
        };

        var emptyPage = new List<Subscription>();

        subscriptionRepository.GetSubscriptionsByChatAsync(
                chatId,
                null,
                Arg.Any<PageRequest>(),
                Arg.Any<CancellationToken>())
            .Returns(firstPage, emptyPage);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork, pageSize: 10);

        var result = await service.GetLinksAsync(chatId);

        Assert.Single(result.Links);
        Assert.Equal(1, result.Size);
        Assert.Equal("https://example.com", result.Links[0].Url);
        Assert.Contains("news", result.Links[0].Tags);
    }

    [Fact]
    public async Task GetLinksAsync_ShouldThrowBadRequestException_WhenChatIdInvalid()
    {
        var chatId = 0L;

        var service = CreateService();

        await Assert.ThrowsAsync<BadRequestException>(() => service.GetLinksAsync(chatId));
    }

    [Fact]
    public async Task GetLinksAsync_ShouldThrowNotFoundException_WhenChatDoesNotExist()
    {
        var chatId = 123;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        chatRepository.ChatExistByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetLinksAsync(chatId));
    }

    [Fact]
    public async Task RemoveLinkAsync_ShouldRemoveSubscriptionAndLink_WhenNoOtherSubscriptionsExist()
    {
        var chatId = 123;
        var request = new RemoveLinkRequest { Url = "https://example.com" };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var transaction = CreateTransaction();

        var subscription = new Subscription
        {
            Id = 1,
            ChatId = chatId,
            LinkId = 100,
            Link = new Link
            {
                Id = 100,
                Url = request.Url
            },
            Tags = new List<Tag>
            {
                new Tag { Name = "dev" }
            }
        };

        chatRepository.ChatExistByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(true);

        subscriptionRepository.GetByChatIdAndUrlAsync(chatId, request.Url, Arg.Any<CancellationToken>())
            .Returns(subscription);

        subscriptionRepository.ExistsForLinkAsync(subscription.LinkId, Arg.Any<CancellationToken>())
            .Returns(false);

        unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>())
            .Returns(transaction);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        var result = await service.RemoveLinkAsync(chatId, request);

        await subscriptionRepository.Received(1)
            .RemoveByChatIdAndUrlAsync(chatId, request.Url, Arg.Any<CancellationToken>());

        await linkRepository.Received(1)
            .RemoveLinkAsync(subscription.LinkId, Arg.Any<CancellationToken>());

        await unitOfWork.Received(2)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await transaction.Received(1)
            .CommitAsync(Arg.Any<CancellationToken>());

        Assert.Equal(request.Url, result.Url);
        Assert.Contains("dev", result.Tags);
    }

    [Fact]
    public async Task RemoveLinkAsync_ShouldRemoveOnlySubscription_WhenOtherSubscriptionsExist()
    {
        var chatId = 123;
        var request = new RemoveLinkRequest { Url = "https://example.com" };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var transaction = CreateTransaction();

        var subscription = new Subscription
        {
            Id = 1,
            ChatId = chatId,
            LinkId = 100,
            Link = new Link
            {
                Id = 100,
                Url = request.Url
            },
            Tags = new List<Tag>()
        };

        chatRepository.ChatExistByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(true);

        subscriptionRepository.GetByChatIdAndUrlAsync(chatId, request.Url, Arg.Any<CancellationToken>())
            .Returns(subscription);

        subscriptionRepository.ExistsForLinkAsync(subscription.LinkId, Arg.Any<CancellationToken>())
            .Returns(true);

        unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>())
            .Returns(transaction);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        await service.RemoveLinkAsync(chatId, request);

        await subscriptionRepository.Received(1)
            .RemoveByChatIdAndUrlAsync(chatId, request.Url, Arg.Any<CancellationToken>());

        await linkRepository.DidNotReceive()
            .RemoveLinkAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());

        await unitOfWork.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await transaction.Received(1)
            .CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveLinkAsync_ShouldThrowBadRequestException_WhenChatIdInvalid()
    {
        var chatId = 0;
        var request = new RemoveLinkRequest { Url = "https://example.com" };

        var service = CreateService();

        await Assert.ThrowsAsync<BadRequestException>(() => service.RemoveLinkAsync(chatId, request));
    }

    [Fact]
    public async Task RemoveLinkAsync_ShouldThrowNotFoundException_WhenChatDoesNotExist()
    {
        var chatId = 123;
        var request = new RemoveLinkRequest { Url = "https://example.com" };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        chatRepository.ChatExistByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(() => service.RemoveLinkAsync(chatId, request));
    }

    [Fact]
    public async Task RemoveLinkAsync_ShouldThrowNotFoundException_WhenSubscriptionDoesNotExist()
    {
        var chatId = 123;
        var request = new RemoveLinkRequest { Url = "https://example.com" };

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();
        var subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        chatRepository.ChatExistByChatIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(true);

        subscriptionRepository.GetByChatIdAndUrlAsync(chatId, request.Url, Arg.Any<CancellationToken>())
            .Returns((Subscription?)null);

        var service = CreateService(linkRepository, chatRepository, subscriptionRepository, unitOfWork);

        await Assert.ThrowsAsync<NotFoundException>(() => service.RemoveLinkAsync(chatId, request));
    }
}