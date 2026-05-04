using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Application.Providers;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Scrapper.Application.DI;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaginationOptions>(configuration.GetSection(PaginationOptions.SectionName));
        services.Configure<LinkProcessingOptions>(configuration.GetSection(LinkProcessingOptions.SectionName));

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<ILinkService, LinkService>();
        services.AddScoped<ILinkUpdateService, LinkUpdateService>();

        services.AddScoped<IUpdateProvider, GitHubUpdateProvider>();
        services.AddScoped<IUpdateProvider, StackOverflowUpdateProvider>();

        services.AddScoped<ILinkProcessor, LinkProcessor>();

        services.AddScoped<IReportFormatter, ReportFormatter>();
        services.AddScoped<INotificationFormatter, NotificationFormatter>();

        services.AddScoped<IReportBuilderService, ReportBuilderService>();
        
        services.AddScoped<IOutboxMessageWriter, OutboxMessageWriter>();

        services.AddScoped<IReportDispatchService, ReportDispatchService>();
        services.AddScoped<INotificationDispatchService, NotificationDispatchService>();
        services.AddScoped<IOutboxMessageDispatchService, OutboxMessageDispatchService>();
        return services;
    }
}
