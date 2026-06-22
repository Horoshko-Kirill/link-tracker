using LinkTracker.AiAgent.Application.DI;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.DI;
using LinkTracker.AiAgent.Infrastructure.DI;
using LinkTracker.AiAgent.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AiAgentOptions>(
    builder.Configuration.GetSection(AiAgentOptions.SectionName));

builder.Services.AddOpenApi();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppMetrics();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<RedMetricsMiddleware>();

app.MapPrometheusScrapingEndpoint();

app.Run();
