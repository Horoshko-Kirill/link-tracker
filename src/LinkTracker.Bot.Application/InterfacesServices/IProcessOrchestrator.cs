namespace LinkTracker.Bot.Application.InterfacesServices;

public interface IProcessOrchestrator
{
    public Task<string> HandleMessageAsync(long chatId, string? message, CancellationToken cancellationToken = default);
}
