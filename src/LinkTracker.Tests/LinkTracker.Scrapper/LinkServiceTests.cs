using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Scrapper;

public class LinkServiceTests
{
    [Fact]
    public async Task AddLinkAsync_ShouldAdd()
    {

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        var chatId = 123;

        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(true);
        linkRepository.LinkExistAsync(chatId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var linkService = new LinkService(linkRepository, chatRepository);

        var addLinkRequest = new AddLinkRequest { Url = string.Empty };

        await linkService.AddLinkAsync(chatId, addLinkRequest);

        await linkRepository.Received(1).AddAsync(Arg.Any<Link>(), Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task AddLinkAsync_ShouldBadRequestException()
    {
        var chatId = 0;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        var linkService = new LinkService(linkRepository, chatRepository);

        var addLinkRequest = new AddLinkRequest { Url = string.Empty };

        await Assert.ThrowsAsync<BadRequestException>(() => linkService.AddLinkAsync(chatId, addLinkRequest));
    }

    [Fact]
    public async Task AddLinkAsync_ShouldNotFoundException()
    {
        var chatId = 123;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(false);

        var linkService = new LinkService(linkRepository, chatRepository);

        var addLinkRequest = new AddLinkRequest { Url = string.Empty };

        await Assert.ThrowsAsync<NotFoundException>(() => linkService.AddLinkAsync(chatId, addLinkRequest));
    }

    [Fact]
    public async Task AddLinkAsync_ShouldConflictException()
    {
        var chatId = 123;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(true);
        linkRepository.LinkExistAsync(chatId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        var linkService = new LinkService(linkRepository, chatRepository);

        var addLinkRequest = new AddLinkRequest { Url = string.Empty };

        await Assert.ThrowsAsync<ConflictException>(() => linkService.AddLinkAsync(chatId, addLinkRequest));
    }

    [Fact]
    public async Task GetLinksAsync_ShouldGetLinks()
    {
        var chatId = 123;
        var url = "https://1";

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(true);

        var links = new List<Link>
        {
            new Link
            {
                Id = 1,
                Subscriptions = new List<Subscription>
                {
                    new Subscription
                    {
                        ChatId = chatId,
                    }
                },
                Url = url
            }
        };

        linkRepository.GetLinksByChatAsync(chatId, null, Arg.Any<CancellationToken>()).Returns(links);

        var response = new ListLinksResponse
        {
            Links = new List<LinkResponse>
            {
                new LinkResponse
                {
                    ChatId = chatId,
                    Url = url,
                }
            },
            Size = 1
        };

        var linkService = new LinkService(linkRepository, chatRepository);

        var result = await linkService.GetLinksAsync(chatId, null);

        Assert.Equal(response.Links[0].ChatId, result.Links[0].ChatId);
        Assert.Equal(response.Links[0].Url, result.Links[0].Url);
        Assert.Equal(response.Size, result.Size);
    }


    [Fact]
    public async Task GetLinksAsync_ShouldBadRequestException()
    {
        var chatId = 0;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        var linkService = new LinkService(linkRepository, chatRepository);

        await Assert.ThrowsAsync<BadRequestException>(() => linkService.GetLinksAsync(chatId, null));
    }

    [Fact]
    public async Task GetLinksAsync_ShouldNotFoundException()
    {
        var chatId = 123;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(false);

        var linkService = new LinkService(linkRepository, chatRepository);

        await Assert.ThrowsAsync<NotFoundException>(() => linkService.GetLinksAsync(chatId, null));
    }

    [Fact]
    public async Task RemoveLinksAsync_ShouldRemoveLink()
    {
        var chatId = 123;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(true);
        linkRepository.LinkExistAsync(chatId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        var link = new Link
        {
            Url = string.Empty,
            Subscriptions = new List<Subscription>
            {
                new Subscription
                {
                    ChatId = chatId,
                }
            }
        };

        linkRepository.GetLinkAsync(chatId, Arg.Any<string>()).Returns(link);

        var linkService = new LinkService(linkRepository, chatRepository);

        var removeLinkRequest = new RemoveLinkRequest { Url = string.Empty };

        await linkService.RemoveLinkAsync(chatId, removeLinkRequest);

        await linkRepository.Received(1).RemoveLinkAsync(chatId, Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveLinksAsync_ShouldBadRequestException()
    {
        var chatId = 0;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        var linkService = new LinkService(linkRepository, chatRepository);

        var removeLinkRequest = new RemoveLinkRequest { Url = string.Empty };

        await Assert.ThrowsAsync<BadRequestException>(() => linkService.RemoveLinkAsync(chatId, removeLinkRequest));
    }

    [Fact]
    public async Task RemoveLinksAsync_ShouldNotFoundException()
    {
        var chatId = 123;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(false);

        var linkService = new LinkService(linkRepository, chatRepository);

        var removeLinkRequest = new RemoveLinkRequest { Url = string.Empty };

        await Assert.ThrowsAsync<NotFoundException>(() => linkService.RemoveLinkAsync(chatId, removeLinkRequest));
    }

    [Fact]
    public async Task RemoveLinksAsync_ShouldConflictException()
    {
        var chatId = 123;

        var linkRepository = Substitute.For<ILinkRepository>();
        var chatRepository = Substitute.For<IChatRepository>();

        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(true);
        linkRepository.LinkExistAsync(chatId, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var linkService = new LinkService(linkRepository, chatRepository);

        var removeLinkRequest = new RemoveLinkRequest { Url = string.Empty };

        await Assert.ThrowsAsync<ConflictException>(() => linkService.RemoveLinkAsync(chatId, removeLinkRequest));
    }
}
