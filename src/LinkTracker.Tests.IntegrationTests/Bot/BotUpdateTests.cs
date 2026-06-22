
using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Tests.IntegrationTests.Clients;
using LinkTracker.Tests.IntegrationTests.Fixtures;

namespace LinkTracker.Tests.IntegrationTests.Bot;

[Collection("Integration")]
public class BotUpdateTests
{
    private readonly BotClient _bot;

    public BotUpdateTests(TestEnvironment env)
    {
        var clients = new TestClientsFactory(env);
        _bot = clients.Bot;
    }

    [Fact]
    public async Task ValidUpdate_ReturnsOk()
    {
        var update = new LinkUpdate
        {
            Url = "https://github.com/test",
            Description = "update",
            ChatIds = new List<long> { 1 }
        };

        var exception = await Record.ExceptionAsync(() =>
            _bot.PostUpdateAsync(update));

        Assert.Null(exception);
    }

    [Fact]
    public async Task InvalidUpdate_ReturnsError()
    {
        var invalid = new LinkUpdate
        {
            Url = string.Empty,
            Description = null,
            ChatIds = new List<long>()
        };

        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() =>
        _bot.PostUpdateAsync(invalid));
    }
}
