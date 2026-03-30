using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Telegram;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot;

public class CommandDispatcherTests
{
    /// <summary>
    /// Позитивные тест на вызов корректной комманды
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DispatchAsync_ShouldCallCorrectCommand()
    {
        var client = Substitute.For<ITelegramClient>();
        var start = Substitute.For<ICommand>();
        start.Name.Returns("/start");

        var unknown = Substitute.For<ICommand>();
        unknown.Name.Returns(string.Empty);

        var logger = Substitute.For<ILogger<CommandDispatcher>>();

        var commands = new List<ICommand> { start, unknown };
        var dispatcher = new CommandDispatcher(commands, logger);

        await dispatcher.DispatchAsync("/start", 123, CancellationToken.None);

        await start.Received(1).ExecuteAsync(123, Array.Empty<String>(), Arg.Any<CancellationToken>());
        await unknown.DidNotReceive().ExecuteAsync(Arg.Any<long>(), Array.Empty<String>(), Arg.Any<CancellationToken>());
    }


    /// <summary>
    /// Негативный тест на вызов несуществующей команды
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DispatchAsync_ShouldCallUnknownCommand_WhenNotFound()
    {
        var start = Substitute.For<ICommand>();
        start.Name.Returns("/start");

        var unknown = Substitute.For<ICommand>();
        unknown.Name.Returns(string.Empty);

        var logger = Substitute.For<ILogger<CommandDispatcher>>();

        var commands = new List<ICommand> { start, unknown };
        var dispatcher = new CommandDispatcher(commands, logger);

        await dispatcher.DispatchAsync("/foobar", 123, CancellationToken.None);

        await unknown.Received(1).ExecuteAsync(123, Array.Empty<String>(), Arg.Any<CancellationToken>());
        await start.DidNotReceive().ExecuteAsync(Arg.Any<long>(), Array.Empty<String>(), Arg.Any<CancellationToken>());
    }
}
