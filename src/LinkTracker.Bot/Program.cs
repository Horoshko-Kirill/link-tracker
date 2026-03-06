using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Clients.Scrapper;
using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Configuration;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Services;
using LinkTracker.Bot.Telegram;
using Microsoft.Extensions.Options;
using LinkTracker.Bot.Application.DI;
using LinkTracker.Bot.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection("Bot"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<ITelegramClient, TelegramClient>();

builder.Services.AddTransient<ICommand, StartCommand>();
builder.Services.AddTransient<ICommand, HelpCommand>();
builder.Services.AddTransient<ICommand, UnknownCommand>();

builder.Services.AddTransient<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddTransient<IMessageRoute, MessageRoute>();

builder.Services.AddHttpClient<IScrapperClient, ScrapperClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ScrapperOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddHostedService<TelegramHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

