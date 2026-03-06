using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.InterfacesRepositories
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> GetAsync(long chatId, CancellationToken cancellationToken = default);
        Task SaveAsync(UserSession userSession, CancellationToken cancellationToken = default);
        Task DeleteAsync(long chatId, CancellationToken cancellationToken = default);
    }
}
