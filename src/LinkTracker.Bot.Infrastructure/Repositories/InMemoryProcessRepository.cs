using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Infrastructure.Repositories
{
    public class InMemoryProcessRepository : IProcessRepository
    {
        private readonly Dictionary<long, Process> _processes = new Dictionary<long, Process>();
        private long _idCounter = 1;

        public Task CancelAsync(long chatId, CancellationToken cancellationToken = default)
        {
            var process = _processes.Values.LastOrDefault(p => p.ChatId == chatId && p.Status == ProcessStatus.Active);

            if (process == null)
            {
                return Task.CompletedTask;
            }

            process.Status = ProcessStatus.Cancelled;

            return Task.CompletedTask;
        }

        public Task CompleteAsync(long chatId, CancellationToken cancellationToken = default)
        {
            var process = _processes.Values.LastOrDefault(p => p.ChatId == chatId && p.Status == ProcessStatus.Active);

            if (process == null)
            {
                return Task.CompletedTask;
            }

            process.Status = ProcessStatus.Completed;

            return Task.CompletedTask;
        }

        public Task CreateAsync(Process process, CancellationToken cancellationToken = default)
        {
            process.Id = _idCounter++;
            _processes[process.Id] = process;
            return Task.CompletedTask;
        }

        public Task<Process?> GetActiveProcessAsync(long chatId, CancellationToken cancellationToken = default)
        {
            var process = _processes.Values.LastOrDefault(p => p.ChatId == chatId && p.Status == ProcessStatus.Active);

            return Task.FromResult(process);
        }
    }
}
