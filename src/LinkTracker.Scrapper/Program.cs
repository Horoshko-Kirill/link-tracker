using LinkTracker.Scrapper.Middleware;
using LinkTracker.Scrapper.Application.DI;
using LinkTracker.Scrapper.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddInfrastructure();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
