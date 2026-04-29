using LinkTracker.Bot.Application.Commands;
using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Bot;

public class UntrackCommandTest
{
    [Fact]
    public async Task ExecuteAsync_ShouldBeAwaitingUntrackLinkConstant()
    {
        var client = Substitute.For<ITelegramClient>();
        var processService = Substitute.For<IProcessService>();
        var scrapperClient = Substitute.For<IScrapperClient>();
        var logger = Substitute.For<ILogger<UntrackCommand>>();

        var command = new UntrackCommand(client, scrapperClient, processService, logger);

        var chatId = 123;

        scrapperClient.ChatExistAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(new ExistChatResponse { ExistChat = true });

        processService.StartProcessAsync(chatId, "Untrack", Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await command.ExecuteAsync(chatId, Array.Empty<String>(), CancellationToken.None);

        await client.Received(1).SendMessageAsync(chatId, OutputHandlerConstants.AwaitingUntrackLinkConstant, Arg.Any<CancellationToken>());
    }
}
