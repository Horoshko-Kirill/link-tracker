using LinkTracker.Scrapper.Application.DI;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Configuration;
using LinkTracker.Scrapper.ExceptionInterceptor;
using LinkTracker.Scrapper.Grpc;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Infrastructure.DI;
using LinkTracker.Scrapper.Middleware;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection("TelegramBot"));

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddInfrastructure();

builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IBotClient, BotClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<BotOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<ScrapperGrpcLinkService>();

app.MapControllers();

app.Run();
