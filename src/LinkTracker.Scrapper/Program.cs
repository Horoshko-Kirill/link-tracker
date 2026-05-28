using LinkTracker.Scrapper.Application.DI;
using LinkTracker.Scrapper.DI;
using LinkTracker.Scrapper.Grpc;
using LinkTracker.Scrapper.Infrastructure.DI;
using LinkTracker.Scrapper.Infrastructure.Options;
using LinkTracker.Scrapper.Middleware;
using LinkTracker.Scrapper.Options;
using Microsoft.Extensions.Options;

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

builder.Services.AddClient(builder.Configuration);

builder.Services.AddAppMetrics();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var kestrelOptions = app.Services.GetRequiredService<IOptions<KestrelOptions>>().Value;

app.UseMiddleware<RedMetricsMiddleware>();

app.MapWhen(ctx => ctx.Connection.LocalPort == kestrelOptions.Port, mainApp =>
{
    mainApp.UseMiddleware<RedMetricsMiddleware>();

    mainApp.UseRouting();
    mainApp.UseEndpoints(endpoints =>
    {
        endpoints.MapGrpcService<ScrapperGrpcLinkService>();
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
