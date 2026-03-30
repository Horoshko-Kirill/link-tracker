using LinkTracker.Scrapper.Application.DI;
using LinkTracker.Scrapper.DI;
using LinkTracker.Scrapper.Grpc;
using LinkTracker.Scrapper.Infrastructure.DI;
using LinkTracker.Scrapper.Middleware;
using LinkTracker.Scrapper.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BotOptions>(
    builder.Configuration.GetSection(BotOptions.SectionName));

builder.Services.Configure<ClientOptions>(
    builder.Configuration.GetSection(ClientOptions.SectionName));

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerGen();

builder.Services.AddGrpc();

var clientOptions = builder.Configuration.GetSection(ClientOptions.SectionName).Get<ClientOptions>();

builder.Services.AddClient(clientOptions);

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
