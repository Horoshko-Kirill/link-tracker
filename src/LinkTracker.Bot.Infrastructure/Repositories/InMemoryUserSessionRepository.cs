using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Infrastructure.Repositories
{
    public class InMemoryUserSessionRepository : IUserSessionRepository
    {
        private readonly Dictionary<long, UserSession> _userSessions = new Dictionary<long, UserSession>();
        private long _idCounter = 1;

        public Task AddAsync(UserSession userSession, CancellationToken cancellationToken = default)
        {
            userSession.Id = _idCounter++;
            _userSessions[userSession.Id] = userSession;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(long chatId, CancellationToken cancellationToken = default)
        {
            var userSession = _userSessions.Values.FirstOrDefault(u => u.ChatId == chatId);

            if (userSession != null)
            {
                _userSessions.Remove(userSession.Id);
            }

            return Task.CompletedTask;
        }

        public Task<UserSession?> GetAsync(long chatId, CancellationToken cancellationToken = default)
        {
            var userSession = _userSessions.Values.FirstOrDefault(u => u.ChatId == chatId);

            return Task.FromResult(userSession);
        }

        public Task SaveAsync(UserSession userSession, CancellationToken cancellationToken = default)
        {
            var oldUserSession = _userSessions.Values.FirstOrDefault(u => u.ChatId == userSession.ChatId);

            if (oldUserSession == null)
            {
                return AddAsync(userSession, cancellationToken);
            }

            _userSessions[oldUserSession.Id] = userSession; 

            return Task.CompletedTask;
        }
    }
}
