namespace LinkTracker.Scrapper.Migrator.Services;

public interface IMigrationRunner
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
