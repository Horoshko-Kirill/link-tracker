using LinkTracker.Bot.Migrator;
using LinkTracker.Bot.Migrator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddMigrator(builder.Configuration);

using var host = builder.Build();

var runner = host.Services.GetRequiredService<IMigrationRunner>();
await runner.RunAsync();
