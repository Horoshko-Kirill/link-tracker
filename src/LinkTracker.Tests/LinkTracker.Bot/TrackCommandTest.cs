using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Telegram;
using LinkTracker.Scrapper.Contracts.Dto;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot;

public class TrackCommandTest
{
    [Fact]
    public async Task ExecuteAsync_ShouldBeAwaitingLinkConstant()
    {
        var client = Substitute.For<ITelegramClient>();
        var processService = Substitute.For<IProcessService>();
        var scrapperClient = Substitute.For<IScrapperClient>();
        var logger = Substitute.For<ILogger<TrackCommand>>();

        var command = new TrackCommand(client, processService, scrapperClient, logger);

        var chatId = 123;

        scrapperClient.ChatExistAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(new ExistChatResponse { ExistChat = true });

        processService.StartProcessAsync(chatId, "Track", Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await command.ExecuteAsync(chatId, Array.Empty<String>(), CancellationToken.None);

        await client.Received(1).SendMessageAsync(chatId, OutputHandlerConstants.AwaitingLinkConstant, Arg.Any<CancellationToken>());
    }

}
