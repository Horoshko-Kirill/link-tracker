using LinkTracker.Bot.Application.InterfacesCommon;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Infrastructure.Database;
using LinkTracker.Bot.Infrastructure.Database.Transaction;
using LinkTracker.Bot.Infrastructure.Options;
using LinkTracker.Bot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Bot.Infrastructure.DI;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services, DatabaseOptions databaseOptions)
    {
        switch (databaseOptions.AccessType)
        {
            case "Sql":
                /*services.AddSingleton(_ =>
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
                break;*/
            case "Orm":
                services.AddDbContext<BotDbContext>(options =>
                {
                    options.UseNpgsql(databaseOptions.ConnectionString);
                });
                
                services.AddScoped<IActionItemRepository, OrmActionItemRepository>();
                services.AddScoped<IProcessRepository, OrmProcessRepository>();
                
                services.AddScoped<IUnitOfWork, EfUnitOfWork>();
                break;
        }

        return services;
    }
}