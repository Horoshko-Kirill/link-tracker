using LinkTracker.Bot.Application.DI;
using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.DI;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Grpc;
using LinkTracker.Bot.Infrastructure.DI;
using LinkTracker.Bot.Middleware;
using LinkTracker.Bot.Options;
using LinkTracker.Bot.Services;
using LinkTracker.Bot.Telegram;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenApi();

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection(BotOptions.SectionName));

builder.Services.Configure<ScrapperOptions>(
    builder.Configuration.GetSection(ScrapperOptions.SectionName));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();

builder.Services.AddSingleton<ITelegramClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var useFake = Environment.GetEnvironmentVariable("UseFakeTelegramClient") == "true"
    || config.GetValue<bool>("UseFakeTelegramClient");

    return useFake
        ? new FakeTelegramClient()
        : new TelegramClient(
            sp.GetRequiredService<IOptions<BotOptions>>(),
            sp.GetRequiredService<ILogger<TelegramClient>>()
        );
});

builder.Services.AddTransient<ICommand, StartCommand>();
builder.Services.AddTransient<ICommand, HelpCommand>();
builder.Services.AddTransient<ICommand, UnknownCommand>();
builder.Services.AddTransient<ICommand, TrackCommand>();
builder.Services.AddTransient<ICommand, UntrackCommand>();
builder.Services.AddTransient<ICommand, ListCommand>();
builder.Services.AddTransient<ICommand, CancelCommand>();

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IMessageRoute, MessageRoute>();

builder.Services.AddGrpc();

var clientOptions = builder.Configuration.GetSection(ClientOptions.SectionName).Get<ClientOptions>();

builder.Services.AddClient(clientOptions);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

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

