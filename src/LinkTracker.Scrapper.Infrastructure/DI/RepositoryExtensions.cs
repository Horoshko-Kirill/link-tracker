using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Infrastructure.Database;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using LinkTracker.Scrapper.Infrastructure.Database.Transaction;
using LinkTracker.Scrapper.Infrastructure.Options;
using LinkTracker.Scrapper.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        var dbOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>();
        
        switch (dbOptions.AccessType)
        {
            case "Sql":
                services.AddSingleton(_ =>
                {
                    var builder = new NpgsqlDataSourceBuilder(dbOptions.ConnectionString);
                    return builder.Build();
                });

                services.AddScoped<SqlSession>();

                services.AddScoped<IChatRepository, SqlChatRepository>();
                services.AddScoped<ILinkRepository, SqlLinkRepository>();
                services.AddScoped<ISubscriptionRepository, SqlSubscriptionRepository>();
                services.AddScoped<ITagRepository, SqlTagRepository>();
                services.AddScoped<IUpdateEventRepository, SqlUpdateEventRepository>();
                services.AddScoped<IChatLinkScanReportRepository, SqlChatLinkScanRepository>();

                services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
                break;
            case "Orm":
                services.AddDbContext<ScrapperDbContext>(options =>
                {
                    options.UseNpgsql(dbOptions.ConnectionString);
                });

                services.AddScoped<IChatRepository, OrmChatRepository>();
                services.AddScoped<ILinkRepository, OrmLinkRepository>();
                services.AddScoped<ISubscriptionRepository, OrmSubscriptionRepository>();
                services.AddScoped<ITagRepository, OrmTagRepository>();
                services.AddScoped<IUpdateEventRepository, OrmUpdateEventRepository>();
                services.AddScoped<IChatLinkScanReportRepository, OrmChatLinkScanRepository>();

                services.AddScoped<IUnitOfWork, EfUnitOfWork>();
                break;
        }

        return services;
    }
}