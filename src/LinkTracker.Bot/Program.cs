using LinkTracker.Bot.Application.DI;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Clients.Scrapper;
using LinkTracker.Bot.Commands;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Configuration;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.ExceptionInterceptor;
using LinkTracker.Bot.Grpc;
using LinkTracker.Bot.Infrastructure.Clients;
using LinkTracker.Bot.Infrastructure.DI;
using LinkTracker.Bot.Middleware;
using LinkTracker.Bot.Services;
using LinkTracker.Bot.Telegram;
using LinkTracker.Scrapper.Contracts.Grpc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection("Bot"));

builder.Services.Configure<ScrapperOptions>(
    builder.Configuration.GetSection("Scrapper"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();

builder.Services.AddSingleton<ITelegramClient, TelegramClient>();

builder.Services.AddTransient<ICommand, StartCommand>();
builder.Services.AddTransient<ICommand, HelpCommand>();
builder.Services.AddTransient<ICommand, UnknownCommand>();
builder.Services.AddTransient<ICommand, TrackCommand>();
builder.Services.AddTransient<ICommand, UntrackCommand>();
builder.Services.AddTransient<ICommand, ListCommand>();
builder.Services.AddTransient<ICommand, CancelCommand>();

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IMessageRoute, MessageRoute>();

builder.Services.AddHttpClient<IScrapperClient, ScrapperClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ScrapperOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);
}); 

/*builder.Services.AddGrpcClient<ScrapperLinkService.ScrapperLinkServiceClient>((sp, o) =>
{
    var options = sp.GetRequiredService<IOptions<ScrapperOptions>>().Value;

    o.Address = new Uri(options.BaseUrl);
});

builder.Services.AddSingleton<IScrapperClient, ScrapperGrpcClient>();*/

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});

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

