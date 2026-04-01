using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Infrastructure.Database;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using LinkTracker.Scrapper.Infrastructure.Database.Transaction;
using LinkTracker.Scrapper.Infrastructure.Options;
using LinkTracker.Scrapper.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services, DatabaseOptions databaseOptions)
    {
        switch (databaseOptions.AccessType)
        {
            case "Sql":
                services.AddSingleton(_ =>
                {
                    var builder = new NpgsqlDataSourceBuilder(databaseOptions.ConnectionString);
                    return builder.Build();
                });

                services.AddScoped<SqlSession>();

                services.AddScoped<IChatRepository, SqlChatRepository>();
                services.AddScoped<ILinkRepository, SqlLinkRepository>();
                services.AddScoped<ISubscriptionRepository, SqlSubscriptionRepository>();
                services.AddScoped<ITagRepository, SqlTagRepository>();

                services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
                break;
            case "Orm":
                services.AddDbContext<ScrapperDbContext>(options =>
                {
                    options.UseNpgsql(databaseOptions.ConnectionString);
                });

                services.AddScoped<IChatRepository, OrmChatRepository>();
                services.AddScoped<ILinkRepository, OrmLinkRepository>();
                services.AddScoped<ISubscriptionRepository, OrmSubscriptionRepository>();
                services.AddScoped<ITagRepository, OrmTagRepository>();

                services.AddScoped<IUnitOfWork, EfUnitOfWork>();
                break;
        }

        return services;
    }
}