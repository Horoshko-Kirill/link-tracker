using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Telegram;
using LinkTracker.Scrapper.Contracts.Dto;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Bot
{
    public class StartCommandTests
    {
        /// <summary>
        /// Положительный тест на вызов команды /start
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExecuteAsync_ShouldBeWelcomeMessage()
        {
            var client = Substitute.For<ITelegramClient>();
            var scrapperClient = Substitute.For<IScrapperClient>();

            var logger = Substitute.For<ILogger<StartCommand>>();

            var command = new StartCommand(client, scrapperClient, logger);
            var chatId = 123;

            scrapperClient.ChatExistAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(new ExistChatResponse { ExistChat = true });

            await command.ExecuteAsync(chatId, Array.Empty<String>(), CancellationToken.None);

            await client.Received(1).SendMessageAsync(
                chatId,
                "Добро пожаловать! Используйте /help для списка команд.",
                Arg.Any<CancellationToken>());
        }
    }
}
