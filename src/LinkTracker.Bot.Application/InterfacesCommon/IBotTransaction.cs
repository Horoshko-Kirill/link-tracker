namespace LinkTracker.Bot.Application.InterfacesCommon;

public interface IBotTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}