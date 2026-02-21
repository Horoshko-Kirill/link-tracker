using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Services;
using LinkTracker.Bot.Telegram;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot;

public class TelegramServiceTests
{
    [Fact]
    public async Task StartAsync_ShouldSetCommands()
    {
        var client = Substitute.For<ITelegramClient>();
        var dispatcher = Substitute.For<ICommandDispatcher>();
        var start = Substitute.For<ICommand>();
        start.Name.Returns("/start");

        var commands = new List<ICommand> { start };
        var service = new TelegramReceivingService(client, dispatcher, commands);

        await service.StartAsync(CancellationToken.None);

        await client.Received(1).SetCommandsAsync(commands, Arg.Any<CancellationToken>());
    }
}
