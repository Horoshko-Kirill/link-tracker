using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }
    public async Task DeleteChatAsync(long chatId, CancellationToken cancellation = default)
    {
        if (!(await _chatRepository.ChatExistAsync(chatId)))
        {
            throw new NotFoundException("Чат не существует");
        }

        await _chatRepository.RemoveChatAsync(chatId, cancellation);
    }

    public async Task RegisterChatAsync(long chatId, CancellationToken cancellationToken = default)
    {

        if (chatId <= 0)
        {
            throw new BadRequestException("Неверный id чата");
        }

        if (await _chatRepository.ChatExistAsync(chatId))
        {
            throw new ConflictException("Чат уже существует");
        }

        var chat = new Chat
        {
            ChatId = chatId
        };

        await _chatRepository.AddAsync(chat, cancellationToken);
    }
}
