using LinkTracker.AiAgent.Application.DI;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AiAgentOptions>(
    builder.Configuration.GetSection(AiAgentOptions.SectionName));

builder.Services.AddOpenApi();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
