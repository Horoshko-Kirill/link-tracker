using System.Net;
using LinkTracker.Scrapper.Application.DI;
using LinkTracker.Scrapper.DI;
using LinkTracker.Scrapper.Grpc;
using LinkTracker.Scrapper.Infrastructure.DI;
using LinkTracker.Scrapper.Infrastructure.Options;
using LinkTracker.Scrapper.Middleware;
using LinkTracker.Scrapper.Options;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection(BotOptions.SectionName));

builder.Services.Configure<ClientOptions>(
    builder.Configuration.GetSection(ClientOptions.SectionName));

builder.Services.Configure<ResilienceOptions>(
    builder.Configuration.GetSection(ResilienceOptions.SectionName));

builder.Services.Configure<KestrelOptions>(
    builder.Configuration.GetSection(KestrelOptions.SectionName));

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerGen();

builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrelWithProtocol();

var clientOptions = builder.Configuration.GetSection(ClientOptions.SectionName).Get<ClientOptions>();

builder.Services.AddClient(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPrometheusScrapingEndpoint();

app.MapGrpcService<ScrapperGrpcLinkService>();

app.MapControllers();

app.Run();
