namespace LinkTracker.Bot.Migrator.Services;

public interface IMigrationRunner
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
