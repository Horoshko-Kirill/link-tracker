using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Telegram;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot;

public class UnknownCommandTests
{
    /// <summary>
    /// Положительный тест на вызов неизвестной команды 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ExecuteAsync_ShouldBeUnknownMessage()
    {
        var client = Substitute.For<ITelegramClient>();
        var command = new UnknownCommand(client);
        var chatId = 123;

        await command.ExecuteAsync(chatId);

        await client.Received(1).SendMessageAsync(
            chatId,
            "Неизвестная команда. Используйте /help.",
            Arg.Any<CancellationToken>());
    }
}
