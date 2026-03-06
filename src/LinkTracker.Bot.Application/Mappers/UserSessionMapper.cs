using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.Mappers;

public static class UserSessionMapper
{
    public static UserSessionDto ToDto(UserSession userSession)
    {
        var result = new UserSessionDto
        {
            ChatId = userSession.ChatId,
            State = userSession.State.ToString(),
            PendingLink = userSession.PendingLink
        };

        return result;
    }

    public static UserSession ToDomain(UserSessionDto userSessionDto)
    {
        var result = new UserSession
        {
            ChatId = userSessionDto.ChatId,
            State = Enum.Parse<UserState>(userSessionDto.State),
            PendingLink = userSessionDto.PendingLink
        };

        return result;
    }
}
