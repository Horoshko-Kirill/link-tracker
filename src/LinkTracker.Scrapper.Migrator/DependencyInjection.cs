using LinkTracker.Scrapper.Infrastructure.Options;
using LinkTracker.Scrapper.Migrator.Services;
using LinkTracker.Scrapper.Migrator.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Scrapper.Migrator;

public static class DependencyInjection
{
    public static IServiceCollection AddMigrator(this IServiceCollection services, IConfiguration configuration)
    {
        services
           .AddOptions<DatabaseOptions>()
           .Bind(configuration.GetSection(DatabaseOptions.SectionName))
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.ConnectionString),
               "DatabaseOptions:ConnectionString must be configured")
           .ValidateOnStart();

        services.AddSingleton<IMigrationRunner, MigrationRunner>();

        return services;
    }
}
