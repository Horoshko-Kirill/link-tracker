using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Constans;
using LinkTracker.Bot.Telegram;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot;

public class HelpCommandTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldBeListAllCommand()
    {
        var client = Substitute.For<ITelegramClient>();
        var command = new HelpCommand(client);
        var chatId = 123;

        await command.ExecuteAsync(chatId);

        await client.Received(1).SendMessageAsync(chatId, HelpConstants.constants, Arg.Any<CancellationToken>());
    }
}
