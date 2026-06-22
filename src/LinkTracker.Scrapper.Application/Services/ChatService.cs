using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChatService(IChatRepository chatRepository, IUnitOfWork unitOfWork)
    {
        _chatRepository = chatRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task DeleteChatAsync(long chatId, CancellationToken cancellation = default)
    {
        if (!(await _chatRepository.ChatExistByChatIdAsync(chatId)))
        {
            throw new NotFoundException("Чат не существует");
        }

        await _chatRepository.RemoveByChatIdAsync(chatId, cancellation);
        await _unitOfWork.SaveChangesAsync(cancellation);
    }

    public async Task<ExistChatResponse> ExistChatAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var response = new ExistChatResponse
        {
            ExistChat = await _chatRepository.ChatExistByChatIdAsync(chatId)
        };

        return response;
    }

    public async Task RegisterChatAsync(long chatId, CancellationToken cancellationToken = default)
    {

        if (chatId <= 0)
        {
            throw new BadRequestException("Неверный id чата");
        }

        if (await _chatRepository.ChatExistByChatIdAsync(chatId))
        {
            throw new ConflictException("Вы уже зарегистрированы. Используйте /help для списка команд");
        }

        var chat = new Chat
        {
            ChatId = chatId
        };

        await _chatRepository.AddChatAsync(chat, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
