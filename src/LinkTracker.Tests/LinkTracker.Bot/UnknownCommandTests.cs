using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Telegram;
using LinkTracker.Scrapper.Contracts.Dto;
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
        var scrapperClient = Substitute.For<IScrapperClient>();
        var command = new UnknownCommand(client, scrapperClient);
        var chatId = 123;

        scrapperClient.ChatExistAsync(chatId, Arg.Any<CancellationToken>())
        .Returns(new ExistChatResponse { ExistChat = true });

        await command.ExecuteAsync(chatId, Array.Empty<String>());

        await client.Received(1).SendMessageAsync(
            chatId,
            "Неизвестная команда. Используйте /help.",
            Arg.Any<CancellationToken>());
    }
}
