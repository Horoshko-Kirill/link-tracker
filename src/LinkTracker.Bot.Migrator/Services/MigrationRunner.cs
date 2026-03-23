using DbUp;
using LinkTracker.Bot.Infrastructure.Options;
using LinkTracker.Bot.Migrator.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Migrator.Services;

public class MigrationRunner : IMigrationRunner
{
    private const string ServiceName = "bot";

    private readonly DatabaseOptions _databaseOptions;
    private readonly ILogger<MigrationRunner> _logger;
    public MigrationRunner(IOptions<DatabaseOptions> databaseOptions, ILogger<MigrationRunner> logger)
    {
        _databaseOptions = databaseOptions.Value;
        _logger = logger;
    }

    public Task RunAsync(CancellationToken cancellationToken = default)
    {
        var scriptsPath = MigrationPathResolver.Resolve(AppContext.BaseDirectory, ServiceName);

        _logger.LogInformation("Starting database migration for {ServiceName}", ServiceName);
        _logger.LogInformation("Using migrations path: {ScriptsPath}", scriptsPath);

        EnsureDatabase.For.PostgresqlDatabase(_databaseOptions.ConnectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_databaseOptions.ConnectionString)
            .WithScriptsFromFileSystem(scriptsPath)
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            _logger.LogError(result.Error, "Database migration failed for {ServiceName}", ServiceName);
            throw result.Error;
        }

        _logger.LogInformation("Database migration completed successfully for {ServiceName}", ServiceName);

        return Task.CompletedTask;
    }
}
