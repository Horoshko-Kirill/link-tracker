using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Configuration;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Services;
using LinkTracker.Bot.Telegram;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection("Bot"));

builder.Services.AddSingleton<ITelegramClient, TelegramClient>();

builder.Services.AddTransient<ICommand, StartCommand>();
builder.Services.AddTransient<ICommand, HelpCommand>();

builder.Services.AddTransient<UnknownCommand>();

builder.Services.AddTransient<CommandDispatcher>();

builder.Services.AddSingleton<TelegramReceivingService>();

builder.Services.AddHostedService<TelegramHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

