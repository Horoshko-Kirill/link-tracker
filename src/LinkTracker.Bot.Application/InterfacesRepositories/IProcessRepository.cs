using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.InterfacesRepositories
{
    public interface IProcessRepository
    {
        Task<Process?> GetActiveProcessAsync(long chatId, CancellationToken cancellationToken = default);
        Task CreateAsync(Process process, CancellationToken cancellationToken = default);
        Task CompleteAsync(long chatId, CancellationToken cancellationToken = default);
        Task CancelAsync(long  chatId, CancellationToken cancellationToken = default);
    }
}
