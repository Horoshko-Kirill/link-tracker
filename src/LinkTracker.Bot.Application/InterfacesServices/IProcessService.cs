namespace LinkTracker.Bot.Application.InterfacesServices;

public interface IProcessService
{
    public Task StartProcessAsync(long chatId, string processType, CancellationToken cancellationToken = default);
    public Task CancelProcessAsync(long chatId, CancellationToken cancellationToken = default);
}
