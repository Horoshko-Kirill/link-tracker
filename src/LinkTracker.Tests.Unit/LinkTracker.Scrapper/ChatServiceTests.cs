using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Domain.Models;
using NSubstitute;

namespace LinkTracker.Tests.LinkTracker.Scrapper;

public class ChatServiceTests
{

    [Fact]
    public async Task DeleteChatAsync_ShouldDelete()
    {
        long chatId = 123;

        var chatRepository = Substitute.For<IChatRepository>();
        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(true);

        var chatService = new ChatService(chatRepository);
        await chatService.DeleteChatAsync(chatId);

        await chatRepository.Received(1).RemoveChatAsync(chatId, Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task DeleteChatAsync_ShouldExceptionNotFound()
    {
        long chatId = 123;

        var chatRepository = Substitute.For<IChatRepository>();
        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(false);

        var chatService = new ChatService(chatRepository);


        await Assert.ThrowsAsync<NotFoundException>(() => chatService.DeleteChatAsync(chatId));
    }


    [Fact]
    public async Task RegisterChatAsync_ShouldRegister()
    {
        long chatId = 123;

        var chatRepository = Substitute.For<IChatRepository>();
        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(false);

        var chatService = new ChatService(chatRepository);
        await chatService.RegisterChatAsync(chatId);

        await chatRepository.Received(1).AddAsync(Arg.Any<Chat>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterChatAsync_ShouldBadRequestException()
    {
        long chatId = 0;

        var chatRepository = Substitute.For<IChatRepository>();

        var chatService = new ChatService(chatRepository);

        await Assert.ThrowsAsync<BadRequestException>(() => chatService.RegisterChatAsync(chatId));
    }

    [Fact]
    public async Task RegisterChatAsync_ShouldConflictException()
    {
        long chatId = 123;
        var chatRepository = Substitute.For<IChatRepository>();
        chatRepository.ChatExistAsync(chatId, Arg.Any<CancellationToken>()).Returns(true);

        var chatService = new ChatService(chatRepository);

        await Assert.ThrowsAsync<ConflictException>(() => chatService.RegisterChatAsync(chatId));
    }
}
