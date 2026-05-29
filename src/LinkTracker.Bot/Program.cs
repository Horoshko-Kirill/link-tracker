using LinkTracker.Bot.Application.Commands;
using LinkTracker.Bot.Application.Commands.Interfaces;
using LinkTracker.Bot.Application.DI;
using LinkTracker.Bot.DI;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Grpc;
using LinkTracker.Bot.Infrastructure.DI;
using LinkTracker.Bot.Infrastructure.Options;
using LinkTracker.Bot.Middleware;
using LinkTracker.Bot.Options;
using LinkTracker.Bot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenApi();

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection(BotOptions.SectionName));

builder.Services.Configure<ScrapperOptions>(
    builder.Configuration.GetSection(ScrapperOptions.SectionName));

builder.Services.Configure<ResilienceOptions>(
    builder.Configuration.GetSection(ResilienceOptions.SectionName));

builder.Services.Configure<KestrelOptions>(
    builder.Configuration.GetSection(KestrelOptions.SectionName));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.Decorate<ICommandDispatcher, CommandDispatcherMetricsDecorator>();
builder.Services.AddScoped<IMessageRoute, MessageRoute>();

builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrelWithProtocol();

builder.Services.AddClient(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<TelegramHostedService>();

builder.Services.AddAppMetrics();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var kestrelOptions = app.Services.GetRequiredService<IOptions<KestrelOptions>>().Value;

app.MapWhen(ctx => ctx.Connection.LocalPort == kestrelOptions.Port, mainApp =>
{
    mainApp.UseMiddleware<RedMetricsMiddleware>();

    mainApp.UseRouting();
    mainApp.UseEndpoints(endpoints =>
    {
        endpoints.MapGrpcService<BotGrpcUpdateService>();
        endpoints.MapControllers();
    });
});

app.MapWhen(ctx => ctx.Connection.LocalPort == kestrelOptions.MetricsPort, metricsApp =>
{
    metricsApp.UseRouting();
    metricsApp.UseEndpoints(endpoints =>
    {
        endpoints.MapPrometheusScrapingEndpoint();
    });
});

app.Run();

