using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Application.InterfacesServices;

public interface IUserSessionService
{
    Task CreateAsync(long chatId, CancellationToken cancellationToken = default);
    Task<UserSessionDto?> GetAsync(long chatId, CancellationToken cancellationToken = default);
    Task SetStateAsync(long chatId, string state, CancellationToken cancellationToken = default);
    Task SetPendingLinkAsync(long chatId, string link, CancellationToken cancellationToken = default);
    Task ResetAsync(long chatId, CancellationToken cancellationToken = default);
}
