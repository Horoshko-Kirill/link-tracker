using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.Handlers.Interfaces;

public interface IActionHandler
{
    public string Action { get; }
    Task HandleAsync(Process process, string? message, CancellationToken cancellationToken = default);
}
