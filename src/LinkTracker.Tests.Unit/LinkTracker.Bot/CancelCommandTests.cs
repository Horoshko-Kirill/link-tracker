using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Telegram;
using LinkTracker.Scrapper.Contracts.Dto;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot;

public class CancelCommandTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldBeSendCancel()
    {
        var client = Substitute.For<ITelegramClient>();
        var processService = Substitute.For<IProcessService>();
        var logger = Substitute.For<ILogger<CancelCommand>>();

        var command = new CancelCommand(client, processService, logger);

        var chatId = 123;

        processService.CancelProcessAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await command.ExecuteAsync(chatId, Array.Empty<String>(), CancellationToken.None);

        await client.Received(1).SendMessageAsync(chatId, "Диалог отменён.", Arg.Any<CancellationToken>());
    }
}
