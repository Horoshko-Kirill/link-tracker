using LinkTracker.Scrapper.Migrator;
using LinkTracker.Scrapper.Migrator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddMigrator(builder.Configuration);

using var host = builder.Build();

var runner = host.Services.GetRequiredService<IMigrationRunner>();
await runner.RunAsync();
