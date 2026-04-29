using LinkTracker.Bot.Application.Commands;
using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.InterfacesClients;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Bot;

public class HelpCommandTests
{
    /// <summary>
    /// Положительный тест на вызов команды /help
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ExecuteAsync_ShouldBeListAllCommand()
    {
        var client = Substitute.For<ITelegramClient>();
        var command = new HelpCommand(client);
        var chatId = 123;

        await command.ExecuteAsync(chatId, Array.Empty<String>());

        await client.Received(1).SendMessageAsync(chatId, HelpConstants.constants, Arg.Any<CancellationToken>());
    }
}
