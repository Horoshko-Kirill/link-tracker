using LinkTracker.Bot.Clients.Scrapper;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Tests.IntegrationTests.Clients;
using LinkTracker.Tests.IntegrationTests.Fixtures;

namespace LinkTracker.Tests.IntegrationTests.Scrapper;

[Collection("Integration")]
public class LinkTests
{
    private readonly ScrapperClient _scrapper;

    public LinkTests(TestEnvironment env)
    {
        var clients = new TestClientsFactory(env);
        _scrapper = clients.Scrapper;
    }

    [Fact]
    public async Task AddAndGetLink()
    {
        await _scrapper.RegisterChatAsync(1);

        var addResponse = await _scrapper.AddLinkAsync(1, new AddLinkRequest
        {
            Url = "https://github.com/test"
        });

        var links = await _scrapper.GetLinksAsync(1);

        Assert.NotNull(links);
        Assert.NotEmpty(links.Links);

        var exists = links.Links.Any(x => x.Url == "https://github.com/test");

        Assert.True(exists);
    }

    [Fact]
    public async Task AddAndDeleteLink()
    {
        await _scrapper.RegisterChatAsync(2);

        var request = new AddLinkRequest
        {
            Url = "https://github.com/test"
        };

        await _scrapper.AddLinkAsync(2, request);

        await _scrapper.RemoveLinkAsync(2, new RemoveLinkRequest
        {
            Url = request.Url
        });

        var links = await _scrapper.GetLinksAsync(2);

        Assert.NotNull(links);
        Assert.Empty(links.Links);
    }

    [Fact]
    public async Task DeleteFromNonExistingChat_ReturnsError()
    {
        await _scrapper.RegisterChatAsync(3);

        var request = new AddLinkRequest
        {
            Url = "https://github.com/test"
        };

        await _scrapper.AddLinkAsync(3, request);

        await Assert.ThrowsAsync<ScrapperApiException>(async () =>
        {
            await _scrapper.RemoveLinkAsync(999, new RemoveLinkRequest
            {
                Url = request.Url
            });
        });
    }

    [Fact]
    public async Task AddLinkToNonExistingChat_ReturnsError()
    {
        await Assert.ThrowsAsync<ScrapperApiException>(async () =>
        {
            await _scrapper.AddLinkAsync(5, new AddLinkRequest
            {
                Url = "https://github.com/test"
            });
        });
    }

    [Fact]
    public async Task DeletedChat_CannotAddLinks()
    {
        await _scrapper.RegisterChatAsync(4);

        await _scrapper.DeleteChatAsync(4);

        await Assert.ThrowsAsync<ScrapperApiException>(async () =>
        {
            await _scrapper.AddLinkAsync(4, new AddLinkRequest
            {
                Url = "https://github.com/test"
            });
        });
    }
}
