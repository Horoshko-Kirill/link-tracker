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

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenApi();

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection(BotOptions.SectionName));

builder.Services.Configure<ScrapperOptions>(
    builder.Configuration.GetSection(ScrapperOptions.SectionName));

builder.Services.Configure<KestrelOptions>(
    builder.Configuration.GetSection(KestrelOptions.SectionName));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();

builder.Services.AddTelegramClient();

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

