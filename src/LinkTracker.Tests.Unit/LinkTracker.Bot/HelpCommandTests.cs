using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Constants;
using LinkTracker.Bot.Telegram;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot;

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
