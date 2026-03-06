namespace LinkTracker.Bot.Application.InterfacesServices;

public interface IProcessService
{
    public Task StartTrackProcessAsync(long chatId, string processType, CancellationToken cancellationToken = default);
}
