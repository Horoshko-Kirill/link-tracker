using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Telegram;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot
{
    public class StartCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldBeWelcomeMessage()
        {
            var client = Substitute.For<ITelegramClient>();
            var command = new StartCommand(client);
            var chatId = 123;

            await command.ExecuteAsync(chatId);

            await client.Received(1).SendMessageAsync(
                chatId,
                "Добро пожаловать! Используйте /help для списка команд.",
                Arg.Any<CancellationToken>());
        }
    }
}
