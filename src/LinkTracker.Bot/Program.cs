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
builder.Services.AddScoped<IMessageRoute, MessageRoute>();

builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrelWithProtocol();

builder.Services.AddClient(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<TelegramHostedService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGrpcService<BotGrpcUpdateService>();

app.MapControllers();

app.Run();

