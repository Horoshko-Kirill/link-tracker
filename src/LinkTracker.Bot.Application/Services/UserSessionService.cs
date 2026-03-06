using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Application.Mappers;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _repository;

    public UserSessionService(IUserSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateAsync(long chatId, CancellationToken cancellationToken = default)
    {
        if (chatId < 0)
        {
            throw new BotException("Неверный id чата");
        }

        var session = new UserSession
        {
            ChatId = chatId,
            State = UserState.Idle
        };

        await _repository.AddAsync(session, cancellationToken);
    }

    public async Task<UserSessionDto?> GetAsync(long chatId, CancellationToken cancellationToken = default)
    {
        if (chatId < 0)
        {
            throw new BotException("Неверный id чата");
        }

        var session = await _repository.GetAsync(chatId, cancellationToken);

        if (session == null)
        {
            throw new SessionNotFoundException("Сессия не найдена");
        }

        var result = UserSessionMapper.ToDto(session);

        return result;
    }

    public async Task ResetAsync(long chatId, CancellationToken cancellationToken = default)
    {
        if (chatId < 0)
        {
            throw new BotException("Неверный id чата");
        }

        var session = await _repository.GetAsync(chatId, cancellationToken);

        if (session == null)
        {
            throw new SessionNotFoundException("Сессия не найдена");
        }

        session.State = UserState.Idle;
        session.PendingLink = null;

        await _repository.SaveAsync(session, cancellationToken);
    }

    public async Task SetPendingLinkAsync(long chatId, string link, CancellationToken cancellationToken = default)
    {
        if (chatId < 0)
        {
            throw new BotException("Неверный id чата");
        }

        var session = await _repository.GetAsync(chatId, cancellationToken);

        if (session == null)
        {
            throw new SessionNotFoundException("Сессия не найдена");
        }

        session.PendingLink = link;

        await _repository.SaveAsync(session, cancellationToken);
    }

    public async Task SetStateAsync(long chatId, string state, CancellationToken cancellationToken = default)
    {
        if (chatId < 0)
        {
            throw new BotException("Неверный id чата");
        }

        var session = await _repository.GetAsync(chatId, cancellationToken);

        if (session == null)
        {
            throw new SessionNotFoundException("Сессия не найдена");
        }

        UserState userState = Enum.Parse<UserState>(state);

        session.State = userState;

        await _repository.SaveAsync(session, cancellationToken);
    }
}
