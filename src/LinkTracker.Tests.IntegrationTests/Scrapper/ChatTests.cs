using LinkTracker.Bot.Clients.Scrapper;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Tests.IntegrationTests.Clients;
using LinkTracker.Tests.IntegrationTests.Fixtures;

namespace LinkTracker.Tests.IntegrationTests.Scrapper;

[Collection("Integration")]
public class ChatTests
{
    private readonly ScrapperClient _scrapper;
    public ChatTests(TestEnvironment env)
    {
        var clients = new TestClientsFactory(env);
        _scrapper = clients.Scrapper;
    }

    [Fact]
    public async Task RegisterChat_ReturnsOk()
    {
        var exception = await Record.ExceptionAsync(() =>
            _scrapper.RegisterChatAsync(1));

        Assert.Null(exception);
    }

    [Fact]
    public async Task DeleteNonExistingChat_Returns404()
    {
        await Assert.ThrowsAsync<ScrapperApiException>(async () =>
        {
            await _scrapper.DeleteChatAsync(1);
        });
    }
}
